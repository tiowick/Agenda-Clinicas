# CRM Para Clínicas Com C# - AspNet Core MVC

# Sistema de CRM com Autenticação, Autorização Hierárquica e Arquitetura em Camadas

# Author: Jeferson Pimentel Sena, Software Engineer.


## 📋 Sumário

1. [Por que essa Arquitetura?](#por-que-essa-arquitetura)
2. [Visão Geral](#visão-geral)
3. [Arquitetura do Projeto](#arquitetura-do-projeto)
4. [Estrutura de Camadas](#estrutura-de-camadas)
5. [Autenticação](#autenticação)
6. [Autorização e Controle de Acesso](#autorização-e-controle-de-acesso)
7. [Fluxo de Requisições](#fluxo-de-requisições)
8. [DTOs e Entidades](#dtos-e-entidades)
9. [Injeção de Dependências](#injeção-de-dependências)
10. [Configuração e Deploy](#configuração-e-deploy)
11. [Regras de Negócio](#regras-de-negócio)

---

## 🏛️ Por que essa Arquitetura?

### O Problema Inicial

Quando você começou a desenvolver o **Agenda 2.0**, precisava de um sistema que:

1. **Crescesse com os requisitos** - Começando com agendamento simples, evoluindo para multi-tenancy
2. **Fosse fácil de manter** - Múltiplas pessoas trabalhando no código
3. **Permitisse testes** - Código testável em cada camada
4. **Separasse responsabilidades** - Sem misturar lógica de BD com lógica de negócio
5. **Escalasse horizontalmente** - Pronto para crescimento

### Por que NÃO Monolítica Tradicional?

**Arquitetura Monolítica (sem camadas):**
```csharp
// ❌ ERRADO: Controller fazendo tudo
public class AgendaController : Controller
{
    [HttpPost]
    public async Task<IActionResult> SaveAgenda([FromForm] Calendario agenda)
    {
        // Valida dados
        if (agenda == null) return BadRequest();
        
        // Executa SQL direto
        using (var conn = new NpgsqlConnection(connStr))
        {
            var cmd = new NpgsqlCommand("INSERT INTO calendarios...", conn);
            cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer, 4);
            // ... 50 linhas de SQL
            await cmd.ExecuteNonQueryAsync();
        }
        
        // Retorna view
        return RedirectToAction("Index");
    }
}
```

**Problemas:**
- 🔴 Controller com 1000+ linhas de código
- 🔴 SQL espalhado em vários controllers
- 🔴 Impossível testar lógica isoladamente
- 🔴 Mudar banco de dados = refatorar tudo
- 🔴 Código duplicado em múltiplos controllers
- 🔴 Difícil onboarding de novos desenvolvedores

### Por que NÃO Microserviços YET?

**Microserviços (complexo demais agora):**
```
┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐
│  Auth Service   │  │ Calendario      │  │ Payment Service │
│  (Port 3001)    │  │ Service         │  │ (Port 3003)     │
│                 │  │ (Port 3002)     │  │                 │
└─────────────────┘  └─────────────────┘  └─────────────────┘
        ↑                    ↑                      ↑
        │ gRPC/HTTP          │ REST API             │ Message Queue
        └────────────────────┴──────────────────────┘
```

**Por que não (agora):**
- 🔴 Complexidade operacional (DevOps, Kubernetes, CI/CD)
- 🔴 Latência entre serviços (sua API é rápida!)
- 🔴 Consistência de dados distribuída
- 🔴 Debugging e monitoramento complexo
- 🔴 Overhead em projeto pequeno/médio

### ✅ A Solução: Arquitetura em Camadas (Clean Architecture)

```
┌─────────────────────────────────┐
│  APRESENTAÇÃO (Controllers)     │  ← Usuário
├─────────────────────────────────┤  
│  APLICAÇÃO (AppServicos)        │  ← Orquestração
├─────────────────────────────────┤
│  DOMÍNIO (Serviços)             │  ← Lógica de Negócio
├─────────────────────────────────┤
│  REPOSITÓRIO/INFRA              │  ← Dados
├─────────────────────────────────┤
│  BANCO DE DADOS                 │  ← Persistência
└─────────────────────────────────┘
```

**Características:**

| Aspecto | Monolítica | **Sua Arquitetura** | Microserviços |
|---------|-----------|-------------------|---------------|
| **Complexidade** | Baixa | **Médio** ✅ | Alta |
| **Escalabilidade** | Difícil | **Fácil** ✅ | Muito fácil |
| **Manutenibilidade** | Baixa | **Excelente** ✅ | Complexa |
| **Testabilidade** | Baixa | **Excelente** ✅ | Excelente |
| **Tempo de deploy** | Rápido | **Rápido** ✅ | Lento |
| **Pronto para produção** | Agora | **Agora** ✅ | Depois |

---

## 🎯 Visão Geral

O **Agenda 2.0** é uma aplicação ASP.NET Core MVC que implementa um sistema completo de gestão de agendamentos com:

- ✅ **Autenticação** baseada em Identity com cookies
- ✅ **Autorização** hierárquica em 7 níveis de permissão
- ✅ **Arquitetura em camadas** seguindo princípios SOLID
- ✅ **Banco de dados PostgreSQL** com procedures SQL
- ✅ **Tratamento centralizado de erros** e exceções
- ✅ **Sistema de Policy hierárquico** (não apenas roles)
- ✅ **DTOs** para transferência segura de dados entre camadas

---

## 📊 Os 4 Pilares da Escalabilidade e Manutenibilidade

### 1️⃣ **Separação de Responsabilidades (SRP - Single Responsibility Principle)**

Cada classe tem UMA responsabilidade, UMA razão para mudar:

**Exemplo REAL do projeto:**

```csharp
// ✅ Responsabilidade 1: Controller - Gerenciar HTTP
public class CalendarioController : BasicController
{
    [HttpPost]
    public async Task<JsonResult> AlterarAgendamentos([FromForm] Calendario dados)
    {
        // APENAS valida entrada e chama AppServicos
        using var app = new CalendarioAppServicos(...);
        await app.CreateOrUpdate(dados);
        return await ResponseJson(ResponseJsonTypes.Success);
    }
}

// ✅ Responsabilidade 2: AppServicos - Orquestração
public class CalendarioAppServicos : BaseAppServicos<Calendario>
{
    // APENAS coordena serviço e repositório
    public CalendarioAppServicos(IUser accessor, IConfiguration config, TransferenciaIdentidadeDTO id)
    {
        ICalendarioRepositorio repo = new CalendarioRepositorio(accessor, config, id);
        ICalendarioServicos servico = new CalendarioServicos(repo, accessor, config, id);
        SetBaseServicos(servico);
    }
}

// ✅ Responsabilidade 3: Serviço - Lógica de Negócio
public class CalendarioServicos : BaseServicos<Calendario>, ICalendarioServicos
{
    // APENAS implementa regras de negócio
    public async Task<RetornoGridPaginado<Calendario>> CarregarGridEnventosCalendario(...)
    {
        var resultado = await _repositorio.CarregarGridEnventosCalendario(...);
        ErrorRepositorio = _repositorio.ErrorRepositorio;
        MessageError = _repositorio.MessageError;
        return resultado;
    }
}

// ✅ Responsabilidade 4: Repositório - Acesso a Dados
public class CalendarioRepositorio : BaseRepositorio<Calendario>
{
    // APENAS executa queries SQL
    public async Task<RetornoGridPaginado<Calendario>> CarregarGridEnventosCalendario(...)
    {
        var query = "SELECT * FROM public.ssp_carregargridagendas(...)";
        var cn = new SqlSystemConnect(ConnectionString);
        return await cn.Query<Calendario>(query);
    }
}
```

**Benefício:**
- 🟢 Você muda lógica de negócio? Afeta APENAS CalendarioServicos
- 🟢 Você muda banco de dados? Afeta APENAS CalendarioRepositorio
- 🟢 Você muda tipo de resposta? Afeta APENAS CalendarioController
- 🟢 ZERO impacto nas outras camadas

### 2️⃣ **Dependency Injection (DI) - Desacoplamento**

Classes NÃO criam suas próprias dependências, recebem prontas:

```csharp
// ❌ ERRADO (Acoplado):
public class CalendarioServicos
{
    private ICalendarioRepositorio repo = new CalendarioRepositorio(); // Acoplado!
}

// ✅ CORRETO (Desacoplado):
public class CalendarioServicos
{
    private ICalendarioRepositorio repo;
    
    public CalendarioServicos(ICalendarioRepositorio repositorio) // Injetado
    {
        repo = repositorio;
    }
}
```

**Por que importa?**

```csharp
// SEM DI: Impossível testar
[Test]
public void TestCalendarioServicos()
{
    var servico = new CalendarioServicos(); // Cria repositório REAL
    // Acessa BD durante teste? 🔴 RUIM
}

// COM DI: Fácil testar com Mock
[Test]
public void TestCalendarioServicos()
{
    var mockRepo = new Mock<ICalendarioRepositorio>();
    mockRepo.Setup(r => r.CarregarGrid(...))
            .Returns(Task.FromResult(fakeData));
    
    var servico = new CalendarioServicos(mockRepo.Object);
    // Testa apenas lógica, sem BD 🟢 BOM
}
```

**Resultado no Agenda 2.0:**
```csharp
// Program.cs: Registra dependências UMA VEZ
builder.Services.AddScoped<IUser, AspNetUser>();
builder.Services.AddScoped<IStoreRoles, StoreRoles>();
builder.Services.AddHttpContextAccessor();

// Controller recebe injetado
public CalendarioController(
    IWebHostEnvironment environment,
    IHttpContextAccessor context,
    IConfiguration configuration,
    SignInManager<IdentityUser> SignInManager,
    UserManager<IdentityUser> UserManager,
    IPrincipal principal,
    IUser user,
    IStoreRoles storeRoles)
    : base(environment, Policy.User, context, ...)
{
    // Todas as dependências prontas para usar
}
```

### 3️⃣ **Abstração via Interfaces - Flexibilidade**

Cada camada depende de interfaces, não de implementações concretas:

```csharp
// INTERFACE (Contrato)
public interface ICalendarioRepositorio : IDisposable
{
    Task<RetornoGridPaginado<Calendario>> CarregarGridEnventosCalendario(
        DataTableSearch search, int start, int draw, int length = 10);
}

// IMPLEMENTAÇÃO (Uma possível forma)
public class CalendarioRepositorio : ICalendarioRepositorio
{
    public async Task<RetornoGridPaginado<Calendario>> CarregarGridEnventosCalendario(...)
    {
        // Usa PostgreSQL
    }
}

// SE MUDAR DE BANCO: Apenas cria nova implementação
public class CalendarioRepositorioMongo : ICalendarioRepositorio
{
    public async Task<RetornoGridPaginado<Calendario>> CarregarGridEnventosCalendario(...)
    {
        // Usa MongoDB
    }
}

// Serviço NÃO MUDA porque trabalha com Interface
public class CalendarioServicos
{
    private readonly ICalendarioRepositorio _repositorio; // ← Interface!
    
    public CalendarioServicos(ICalendarioRepositorio repositorio)
    {
        _repositorio = repositorio; // Pode ser PostgreSQL ou MongoDB
    }
}
```

**Benefício:**
- 🟢 Trocar PostgreSQL por MongoDB? Cria `CalendarioRepositorioMongo` implementando interface
- 🟢 Testes? Cria `CalendarioRepositorioMock` para testes
- 🟢 Serviço, Controller, AppServicos? NÃO MUDA NADA

### 4️⃣ **Reutilização via Classes Base (DRY - Don't Repeat Yourself)**

Código comum em classes base, especializado em subclasses:

```csharp
// BASE - CRUD padrão
public abstract class BaseRepositorio<TEntity> : IBaseRepositorio<TEntity>
{
    public async Task<long> CreateOrUpdate(TEntity entity) { /* ... */ }
    public async Task<bool> Delete(long id) { /* ... */ }
    public async Task<IEnumerable<TEntity>> GetData() { /* ... */ }
}

// ESPECIALIZADA - Operações específicas
public class CalendarioRepositorio : BaseRepositorio<Calendario>, ICalendarioRepositorio
{
    // Herda CRUD de BaseRepositorio
    // Adiciona CarregarGridEnventosCalendario
    
    public async Task<RetornoGridPaginado<Calendario>> CarregarGridEnventosCalendario(...) 
    {
        // Específico de Calendario
    }
}

// OUTRA ESPECIALIZADA
public class PerfilRepositorio : BaseRepositorio<Perfil>, IPerfilRepositorio
{
    // Herda CRUD de BaseRepositorio
    // Adiciona CarregarPerfisPorEmpresa
    
    public async Task<List<Perfil>> CarregarPerfisPorEmpresa(long idEmpresa) 
    {
        // Específico de Perfil
    }
}
```

**Resultado:** 
- 🟢 Código CreateOrUpdate escrito UMA VEZ em BaseRepositorio
- 🟢 CalendarioRepositorio, PerfilRepositorio, etc. herdam AUTOMATICAMENTE
- 🟢 Mudar lógica de CreateOrUpdate? Muda em UM lugar, todos herdam

---

## 📈 Como Escala com Essa Arquitetura?

### Cenário 1: Adicionar Nova Funcionalidade (Hoje)

```
1. Criar entidade: Cupom.cs
   ↓
2. Criar interface: ICupomRepositorio.cs, ICupomServicos.cs
   ↓
3. Criar repositório: CupomRepositorio.cs (herda BaseRepositorio)
   ↓
4. Criar serviço: CupomServicos.cs (herda BaseServicos)
   ↓
5. Criar AppServicos: CupomAppServicos.cs
   ↓
6. Criar controller: CupomController.cs (herda BasicController)
   ↓
7. Pronto! Tudo funciona
```

**Tempo:** ~2 horas para funcionalidade completa (CRUD + autorização)

### Cenário 2: Mudar Banco de Dados (Escala)

```
ANTES (PostgreSQL):
CalendarioRepositorio extends BaseRepositorio
    └─ Executa: SELECT * FROM ssp_carregargridagendas(...)

DEPOIS (MySQL):
CalendarioRepositorio extends BaseRepositorio
    └─ Executa: CALL sp_carregargridagendas(...)
```

**Mudança:** APENAS na classe Repositorio
**Impacto:** ZERO em Serviços, AppServicos, Controllers

### Cenário 3: Adicionar Novo Tipo de Autenticação (OAuth2)

```csharp
// ANTES: Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(...);

// DEPOIS: OAuth2 + Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(...)
    .AddGoogle(...);

// Código INTEIRO da aplicação funciona igual
// Controllers, Services, Repositórios? NÃO MUDAM
```

### Cenário 4: Dividir em Microserviços (Futuro)

```
HOJE:
Agenda.sln
├─ Agenda (MVC)
├─ Agenda.Aplicacao
├─ Agenda.Dominio
└─ Agenda.Repositorio

FUTURO:
AuthService.sln          ← Microserviço 1
├─ Auth.API
├─ Auth.Aplicacao
├─ Auth.Dominio
└─ Auth.Repositorio

AgendaService.sln        ← Microserviço 2
├─ Agenda.API
├─ Agenda.Aplicacao
├─ Agenda.Dominio
└─ Agenda.Repositorio
```

**Vantagem:** Código já está estruturado para isso!
**Esforço:** Extrair serviço é relativamente simples

---

## 🛠️ Por que Fácil de Manter?

### 1. Encontrar Bug é Simples

```csharp
// Bug: "Agendamentos não estão filtrando por empresa"

// Stack trace aponta: CalendarioController.CarregarGridEnventosCalendario

// Você já sabe:
// ✅ Bug está em CalendarioRepositorio (acesso a dados)
// ✅ Não está em CalendarioServicos (lógica ok)
// ✅ Não está em CalendarioController (recebe parâmetro correto)

// Abre: CalendarioRepositorio.cs
// Vê: 
var query = "SELECT * FROM calendarios WHERE id_vendedor = @vendedor";
// ❌ FALTA: "AND id_empresa = @empresa"

// Fix: Adiciona filtro de empresa
var query = "SELECT * FROM calendarios WHERE id_vendedor = @vendedor AND id_empresa = @empresa";
```

**Resultado:** Bug encontrado e fixado em 5 minutos

### 2. Onboarding de Novos Desenvolvedores

**Novo dev chega:**

```
Semana 1:
- Entende que tem 4 camadas
- Entende que tudo segue padrão Base*
- Entende que Controllers herdam BasicController

Semana 2:
- Cria primeira funcionalidade (seguindo padrão)
- CalendarioController → CalendarioAppServicos → CalendarioServicos → CalendarioRepositorio

Semana 3:
- Já está criando features independentemente
```

**Comparação com monolítica:**
- Monolítica: "Onde coloco esse código?" (ambiguidade)
- Sua arquitetura: "Vai no CalendarioRepositorio" (claro)

### 3. Code Review é Fácil

```
PR Review:
1. Alterou Controller? ✅ Verifica apenas lógica HTTP
2. Alterou AppServicos? ✅ Verifica apenas orquestração
3. Alterou Servicos? ✅ Verifica apenas regras de negócio
4. Alterou Repositorio? ✅ Verifica apenas query/BD

SEM essa arquitetura:
1. Alterou tudo junto em 1 arquivo? 😫 Code review impossível
```

### 4. Testes são Naturais

```csharp
// Testar apenas Serviço (sem BD)
[Test]
public async Task TestCarregarGridComFiltro()
{
    var mockRepo = new Mock<ICalendarioRepositorio>();
    mockRepo.Setup(r => r.CarregarGridEnventosCalendario(...))
            .Returns(Task.FromResult(dadosFake));
    
    var servico = new CalendarioServicos(mockRepo.Object, null, null, id);
    var resultado = await servico.CarregarGridEnventosCalendario(...);
    
    Assert.AreEqual(2, resultado.recordsTotal);
}
```

**Sem DI/Interfaces:** Impossível, porque repositório criava BD real

---

## 💰 ROI (Return on Investment) da Arquitetura

| Métrica | Monolítica | **Sua Arquitetura** |
|---------|-----------|-------------------|
| **Tempo adicionar feature** | 1 semana | **2 dias** ⏱️ |
| **Tempo encontrar bug** | 3 horas | **30 minutos** 🐛 |
| **Tempo refatoração** | 2 semanas | **3 dias** 🔧 |
| **Tempo onboarding dev** | 3 semanas | **1 semana** 👨‍💻 |
| **Cobertura de testes** | ~20% | **~80%** ✅ |
| **Custo de mudança de BD** | Alto | **Baixo** 💰 |

---



---

## 🏗️ Arquitetura do Projeto

O projeto segue uma arquitetura de **4 camadas** separadas em diferentes projetos:

```
┌─────────────────────────────────────────────────────────────┐
│                 Agenda (Apresentação)                        │
│              Controllers, Views, Areas, Models               │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│              Agenda.Aplicacao (Aplicação)                    │
│    AppServicos, BaseAppServicos, Interfaces de Serviços     │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│              Agenda.Dominio (Domínio)                        │
│   Entidades, DTOs, Enums, Interfaces, Serviços de Negócio  │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│            Agenda.Repositorio + Agenda.Infra               │
│        Repositórios, Acesso a Dados, Conexões BD           │
└─────────────────────────────────────────────────────────────┘
```

---

## 📦 Estrutura de Camadas

### 1️⃣ **Camada de Apresentação (Agenda)**

**Responsabilidades:**
- Gerenciar HTTP requests/responses
- Renderizar Views (Razor)
- Coordenar fluxos de usuário
- Aplicar políticas de autorização

**Componentes principais:**

#### **Controllers**
- `HomeController`: Acesso público, login e redirecionamentos
- `BasicController`: Classe base abstrata para todos os demais controllers
- `AcessosController`: Gerenciamento de acessos e permissões
- Outros controllers herdam de `BasicController`

#### **AbstractFactory (Padrão de Design)**

```
AbstractFactory/
├── AspNetUser.cs       → Implementa IUser (usuário autenticado)
└── StoreRoles.cs       → Implementa IStoreRoles (armazenamento de permissões)
```

**AspNetUser** - Abstrai o acesso ao usuário autenticado:
```csharp
public class AspNetUser : IUser
{
    // Obtém o nome do usuário autenticado
    public string Name { get; }
    
    // Verifica se está autenticado
    public bool IsAuthenticated()
    
    // Obtém claims (permissões, roles, etc)
    public IEnumerable<Claim> GetClaimsIdentity()
    
    // Retorna ClaimsIdentity completo
    public ClaimsIdentity ClaimsIdentity { get; }
}
```

**StoreRoles** - Gerencia roles e policies do usuário:
```csharp
public sealed class StoreRoles : IStoreRoles
{
    // Lista de roles/papéis do usuário
    public IList<string> Roles { get; set; }
    
    // Verifica se está no nível de policy exigido
    public bool IsInPolicy(Policy roleName)
    
    // Verifica se tem um role específico
    public bool IsInRole(UserRoles roleName)
    
    // Indica se está autorizado
    public bool IsAuthorized { get; set; }
}
```

#### **Areas**
```
Areas/
├── Identity/      → Gerenciamento de identidade e login
├── Basico/        → Funcionalidades básicas
└── Agenda/        → Funcionalidades de agendamento
```

As **Areas** permitem organizar controllers, views e modelos em módulos separados.

#### **Data**
- `ApplicationDbContext`: Contexto EF Core com Identity integrado

#### **Models**
- `ErrorViewModel`: Modelo para exibição de erros
- `LoginViewModel`: Modelo para formulário de login
- `RequestToken`, `ResponseMethodJson`: DTOs de comunicação

---

### 2️⃣ **Camada de Aplicação (Agenda.Aplicacao)**

**Responsabilidades:**
- Orquestrar fluxos de negócio
- Validar dados antes de usar
- Coordenar repositórios e serviços
- Traduzir DTOs entre camadas
- Propagar erros de serviços para controllers

**Componentes principais:**

#### **AppServicosGestaoIdentidade**
Orquestrador de identidade - obtém credenciais do usuário:
```csharp
public class AppServicosGestaoIdentidade : IAppServicosGestaoIdentidade, IDisposable
{
    private IServicosGestaoIdentidade _servicosBase { get; set; } = default!;
    public bool ErrorRepositorio { get; private set; } = default!;
    public string MessageError { get; private set; } = default!;
    
    public AppServicosGestaoIdentidade(IConfiguration? configuration, IUser? accessor)
    {
        // Injeta repositório e serviço
        IRepositorioGestaoIdentidade repositorio = new RepositorioGestaoIdentidade(configuration, accessor);
        _servicosBase = new ServicosGestaoIdentidade(repositorio, accessor, configuration);
    }
    
    // Obtém credenciais do usuário (chama serviço de domínio)
    public async Task<TransferenciaIdentidadeDTO> GetCredenciaisUsuario()
    {
        var _return = await _servicosBase.GetCredenciaisUsuario().ConfigureAwait(true);
        ErrorRepositorio = _servicosBase.ErrorRepositorio;
        MessageError = _servicosBase.MessageError;
        return _return;
    }
    
    // Realiza logout do usuário
    public async Task<TransferenciaIdentidadeDTO> GetLogoutUsuario()
    {
        var _return = await _servicosBase.GetLogoutUsuario().ConfigureAwait(true);
        ErrorRepositorio = _servicosBase.ErrorRepositorio;
        MessageError = _servicosBase.MessageError;
        return _return;
    }
}
```

#### **BaseAppServicos<TEntity>** (Exemplo real: CalendarioAppServicos)
Classe base genérica que orquestra serviços:
```csharp
public class CalendarioAppServicos : BaseAppServicos<Calendario>, ICalendarioAppServicos
{
    private readonly ICalendarioServicos _servico;
    
    public CalendarioAppServicos(
        IUser? accessor, 
        IConfiguration? configuration, 
        TransferenciaIdentidadeDTO identidade)
        : base(accessor, configuration, identidade)
    {
        // Cria repositório
        ICalendarioRepositorio _repositorio = new CalendarioRepositorio(
            _accessor, _configuration, identidade);
        
        // Cria serviço de domínio
        _servico = new CalendarioServicos(
            _repositorio, _accessor, _configuration, identidade);
        
        // Define qual serviço usar para operações CRUD
        SetBaseServicos(_servico);
    }
    
    // Passa a chamada para o serviço de domínio
    public async Task<RetornoGridPaginado<Calendario>> CarregarGridEnventosCalendario(
        DataTableSearch search, int start, int draw, int length = 10)
    {
        return await _servico.CarregarGridEnventosCalendario(
            search, start, draw, length).ConfigureAwait(true);
    }
    
    // Herda de BaseAppServicos: CreateOrUpdate, Delete, GetData, etc.
}
```

**O que o BaseAppServicos faz:**
```csharp
public virtual async Task<long> CreateOrUpdate(TEntity entity)
{
    // 1. Chama o serviço de domínio
    var _return = await _servicosBase.CreateOrUpdate(entity).ConfigureAwait(true);
    
    // 2. Propaga erros de forma padronizada
    ErrorRepositorio = _servicosBase.ErrorRepositorio;
    MessageError = _servicosBase.MessageError;
    
    return _return;
}
```

**Uso real em Controller (CalendarioController):**
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<JsonResult> AlterarAgendamentos([FromForm] Calendario dados)
{
    try
    {
        // 1. Valida dados do formulário
        if (dados == null)
            throw new Exception("Dados do formulário vazio");
        
        var context = new ValidationContext(dados, null, null);
        var validationResults = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(dados, context, validationResults, true);
        
        if (validationResults.Any())
        {
            var _erroMensagem = validationResults.FirstOrDefault()?.ErrorMessage ?? "Erro";
            throw new TratamentoExcecao(_erroMensagem.Traduzir());
        }
        
        // 2. Cria AppServicos com identidade do usuário logado
        using var app = new CalendarioAppServicos(
            base.UserIdentity,      // IUser - usuário autenticado
            base.Configuration,     // IConfiguration - conexão BD
            base.Identidade);       // TransferenciaIdentidadeDTO - dados do usuário
        
        // 3. Chama método de negócio (herda de BaseAppServicos)
        _ = await app.CreateOrUpdate(dados);
        
        // 4. Retorna resposta padronizada
        return await ResponseJson(ResponseJsonTypes.Success);
    }
    catch (TratamentoExcecao e) 
    { 
        return await ResponseJson(ResponseJsonTypes.Error, e.Message); 
    }
    catch (Exception ex) 
    { 
        return await ResponseJson(ResponseJsonTypes.Error, ex.Message); 
    }
}
```

**Fluxo:**
```
Controller → CalendarioAppServicos → CalendarioServicos (Dominio) 
           → CalendarioRepositorio → PostgreSQL
```

---

### 3️⃣ **Camada de Domínio (Agenda.Dominio)**

**Responsabilidades:**
- Conter lógica de negócio
- Abstrair interfaces de repositório e serviço
- Definir enums e políticas
- Orquestrar repositórios

**Componentes principais:**

#### **Enums (IGroupPolicies.cs)**

Define 7 níveis hierárquicos de permissão:

```csharp
public enum UserRoles
{
    User = 1,           // Usuário padrão
    Vendedor = 2,       // Vendedor
    Enfermeira = 3,     // Profissional de saúde
    Gerente = 4,        // Gerente
    Diretor = 5,        // Diretor
    Admin = 6,          // Administrador
    Developer = 7       // Desenvolvedor
}

public enum Policy
{
    User = 1,           // Nivel 1
    Vendedor = 2,       // Nivel 2
    Enfermeira = 3,     // Nivel 3
    Gerente = 4,        // Nivel 4
    Diretor = 5,        // Nivel 5
    Admin = 6,          // Nivel 6
    Developer = 7       // Nivel 7
}
```

**Hierarquia:**
- Usuários com nível SUPERIOR podem acessar rotas de nível INFERIOR
- Um Admin (6) pode acessar recursos exigindo Gerente (4)
- Um Vendedor (2) NÃO pode acessar recursos exigindo Gerente (4)

#### **DTOs (Data Transfer Objects)**

**TransferenciaIdentidadeDTO** - Dados do usuário logado:
```csharp
public class TransferenciaIdentidadeDTO
{
    public long IdVendedorLogado { get; set; }    // ID do vendedor
    public long IdUsuarioLogado { get; set; }     // ID do usuário
    public long IdEmpresaLogado { get; set; }     // ID da empresa
    public string NmUsuarioLogado { get; set; }   // Nome do usuário
    public int AutoAgendamento { get; set; }      // Flag de auto-agendamento
    public bool IsAuthorized { get; set; }        // Está autorizado?
    public string RotaController { get; set; }    // Rota acessada
    public long IdCampanha { get; set; }          // ID da campanha
}
```

#### **Serviços de Domínio (Exemplo real: CalendarioServicos)**

**CalendarioServicos** - Contém lógica de negócio:
```csharp
public sealed class CalendarioServicos 
    : BaseServicos<Calendario>, 
      ICalendarioServicos
{
    private readonly ICalendarioRepositorio _repositorio = default!;
    
    public CalendarioServicos(
        ICalendarioRepositorio repositorio, 
        IUser? accessor, 
        IConfiguration? configuration, 
        TransferenciaIdentidadeDTO identidade)
        : base(repositorio, accessor, configuration, identidade)
    {
        _repositorio = repositorio;
    }
    
    // Carrega grid com paginação
    public async Task<RetornoGridPaginado<Calendario>> CarregarGridEnventosCalendario(
        DataTableSearch search, int start, int draw, int length = 10)
    {
        // 1. Chama repositório
        var _result = await _repositorio.CarregarGridEnventosCalendario(
            search, start, draw, length).ConfigureAwait(true);
        
        // 2. Propaga status de erro
        ErrorRepositorio = _repositorio.ErrorRepositorio;
        MessageError = _repositorio.MessageError;
        
        return _result;
    }
    
    // Herda de BaseServicos: CreateOrUpdate, Delete, GetData, etc.
}
```

**O que BaseServicos faz:**
```csharp
public virtual async Task<long> CreateOrUpdate(TEntity entity)
{
    try
    {
        // 1. Chama repositório
        await _repositorio.CreateOrUpdate(entity);
        
        // 2. Trata erros
        if (_repositorio.ErrorRepositorio)
            throw new Exception(_repositorio.MessageError);
        
        ErrorRepositorio = _repositorio.ErrorRepositorio;
        MessageError = _repositorio.MessageError;
        ID = _repositorio.ID;
        
        return await Task.FromResult(_repositorio.ID).ConfigureAwait(true);
    }
    catch (Exception ex)
    {
        ID = 0;
        ErrorRepositorio = true;
        MessageError = ex.Message;
        throw new TratamentoExcecao(ex);
    }
}
```

#### **Interfaces de Serviço**

**ICalendarioServicos** - Contrato do serviço:
```csharp
public interface ICalendarioServicos : IBaseServicos<Calendario>, IDisposable
{
    Task<RetornoGridPaginado<Calendario>> CarregarGridEnventosCalendario(
        DataTableSearch search, int start, int draw, int length = 10);
}
```

**IBaseServicos<TEntity>** - Operações CRUD padrão:
```csharp
public interface IBaseServicos<TEntity> : IDisposable where TEntity : class
{
    TransferenciaIdentidadeDTO Identidade { get; }
    bool ErrorRepositorio { get; }
    string MessageError { get; }
    long ID { get; set; }
    bool IDCreated { get; set; }
    
    Task<long> CreateOrUpdate(TEntity entity);
    Task<bool> CreateList(IEnumerable<TEntity> entity);
    Task<bool> Delete(long id);
    Task<bool> DeleteList(IEnumerable<long> id);
    Task<IEnumerable<TEntity>> GetData(long id);
    Task<IEnumerable<TEntity>> GetData();
}
```

---

### 4️⃣ **Camada de Repositório & Infraestrutura**

**Responsabilidades:**
- Acessar base de dados PostgreSQL
- Executar queries SQL e Stored Functions
- Gerenciar conexões
- Mapear dados SQL para objetos C#

#### **BaseRepositorio<TEntity>** (Classe abstrata)

**Validações obrigatórias:**
```csharp
public abstract class BaseRepositorio<TEntity> : IDisposable, IBaseRepositorio<TEntity>
{
    public BaseRepositorio(IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
    {
        // 1. Valida se identidade foi passada
        if (identidade == null)
            throw new Exception("Usuário sem permissão: Identidade do Usuário Vazio");
        
        // 2. Valida se usuário passou na validação de policy
        if (!(identidade?.IsAuthorized ?? false))
            throw new Exception("Usuário sem permissão: Nível de Acesso Negado");
        
        // 3. Valida se configuração existe
        if (configuration == null)
            throw new Exception("Usuário sem permissão: Configuração de Acesso Vazio");
        
        // Obtém string de conexão
        ConnectionString = configuration?.GetConnectionString("DefaultConnection")
            ?? "Server=bd1.winsiga.com.br; Port=5432; User Id=postgres; Password=soft@2013; Database=DadosAgendaBTG;";
        
        Identidade = identidade;
    }
}
```

#### **CalendarioRepositorio** (Exemplo real)

**Herda de BaseRepositorio<Calendario>:**
```csharp
public class CalendarioRepositorio : BaseRepositorio<Calendario>, ICalendarioRepositorio
{
    private readonly IUser? _accessor;
    private readonly IConfiguration? _configuration;
    private readonly TransferenciaIdentidadeDTO _identidade;
    
    public CalendarioRepositorio(
        IUser? accessor, 
        IConfiguration? configuration, 
        TransferenciaIdentidadeDTO identidade)
        : base(configuration, identidade)
    {
        _accessor = accessor;
        _configuration = configuration;
        _identidade = identidade;
    }
    
    // Exemplo: Carregar grid com paginação
    public async Task<RetornoGridPaginado<Calendario>> CarregarGridEnventosCalendario(
        DataTableSearch search, int start, int draw, int length = 10)
    {
        try
        {
            // 1. Monta query SQL que chama Stored Procedure do PostgreSQL
            var _query = string.Format(
                "SELECT * FROM public.ssp_carregargridagendas ({0}, {1}, {2}, {3}, '{4}');",
                _identidade.IdVendedorLogado,    // Filtra por vendedor
                _identidade.IdUsuarioLogado,     // Filtra por usuário
                start,                           // Paginação
                length,                          // Quantidade registros
                (search?.value ?? "")?.Trim()    // Busca/filtro
            );
            
            // 2. Executa query usando Dapper (SqlSystemConnect)
            var cn = new SqlSystemConnect(ConnectionString);
            var _result = cn.Query<Calendario>(_query, buffered: true, commandTimeout: 1440);
            
            if (!_result.Any())
                return new RetornoGridPaginado<Calendario>().RetornoVazio(draw);
            
            // 3. Retorna resultado paginado
            var _return = new RetornoGridPaginado<Calendario>
            {
                draw = draw,
                recordsTotal = _result.Count(),
                recordsFiltered = _result.Count(),
                data = _result.ToList()
            };
            
            return await Task.FromResult(_return).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            ErrorRepositorio = true;
            MessageError = ex.Message.Traduzir();
            throw new TratamentoExcecao(MessageError);
        }
    }
}
```

#### **SqlSystemConnect** (Wrapper de conexão)

Utiliza **Dapper** para mapping SQL → C#:
```csharp
public class SqlSystemConnect
{
    private readonly string _connectionString;
    
    public SqlSystemConnect(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    // Query genérica com mapping automático
    public IEnumerable<T> Query<T>(
        string sql, 
        bool buffered = true, 
        int commandTimeout = 1440) where T : class
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            return connection.Query<T>(sql, commandTimeout: commandTimeout, buffered: buffered);
        }
    }
}
```

#### **Padrão de acesso a dados:**

```
Controller (CalendarioController)
    ↓
AppServicos (CalendarioAppServicos)
    ↓
DomainServices (CalendarioServicos)
    ↓
Repositorio (CalendarioRepositorio) → BaseRepositorio
    ↓
SqlSystemConnect → Dapper → PostgreSQL Stored Procedure
    ↓
TransferenciaIdentidadeDTO + Query Result → RetornoGridPaginado<T>
```

---

## 🔐 Autenticação

### Estratégia de Autenticação

O projeto usa **ASP.NET Core Identity** com **Cookies**:

**Configuração em Program.cs:**
```csharp
// Adiciona Identity com Entity Framework
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => 
    options.SignIn.RequireConfirmedAccount = true
)
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddRoles<IdentityRole>()
.AddDefaultTokenProviders();

// Configura autenticação via cookie
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Home/Index";                // Redireciona para login
    options.AccessDeniedPath = "/Home/Privacy";       // Acesso negado
    options.Cookie.HttpOnly = true;                   // Previne XSS
    options.Cookie.SameSite = SameSiteMode.Strict;    // CSRF protection
    options.ExpireTimeSpan = TimeSpan.FromDays(60);   // 60 dias de sessão
    options.SlidingExpiration = true;                 // Renova sessão em cada acesso
});
```

### Políticas de Senha

```csharp
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;              // Mínimo 6 caracteres
    
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(60);
    options.Lockout.MaxFailedAccessAttempts = 5;      // 5 tentativas erradas
    
    options.User.RequireUniqueEmail = true;           // Email único
});
```

### Fluxo de Login

1. **HomeController** recebe requisição `/Home/Index`
2. Usuário submete credenciais
3. **SignInManager** valida contra Identity database
4. **AspNetUser** extrai informações do usuário autenticado
5. **Claims** são adicionados ao cookie (contêm roles/permissions)
6. Usuário é redirecionado para Dashboard ou rota solicitada

---

## 🛡️ Autorização e Controle de Acesso

### Sistema Hierárquico de Policies

A autorização funciona em **2 níveis**:

#### **Nível 1: Role-Based (Tradicional)**

Verificação simples de role:
```csharp
public bool IsInRole(UserRoles roleName)
{
    var _roleName = roleName.ToString().ToLower();
    return Roles.Contains(_roleName);
}
```

#### **Nível 2: Policy-Based Hierárquico (Avançado)**

Permite herança de permissões:
```csharp
public bool IsInPolicy(Policy roleName)
{
    int userMaxPolicy = 0;
    int requiredPolicy = (int)roleName;
    
    // Encontra o nível máximo do usuário
    foreach (var role in Roles)
    {
        var policy = MapRoleNameToPolicy(role);
        if (policy.HasValue && (int)policy > userMaxPolicy)
            userMaxPolicy = (int)policy.Value;
    }
    
    // Compara: usuário precisa estar no mesmo nível ou ACIMA
    IsAuthorized = userMaxPolicy >= requiredPolicy;
    return IsAuthorized;
}
```

**Exemplo de Hierarquia:**

```
Developer (7) ──┐
                ├──► Admin (6) ──┐
                                ├──► Diretor (5) ──┐
                                                  ├──► Gerente (4) ──┐
                                                                    ├──► Enfermeira (3) ──┐
                                                                                        ├──► Vendedor (2) ──┐
                                                                                                          ├──► User (1)

✅ Admin PODE acessar rotas que exigem Gerente (4)
❌ Vendedor NÃO PODE acessar rotas que exigem Gerente (4)
✅ Developer PODE acessar TUDO
```

### Atributos de Autorização

#### **Global:**
```csharp
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    
    // Toda action é protegida por padrão
    options.Filters.Add(new AuthorizeFilter(policy));
});
```

#### **Por Controller:**
```csharp
[Authorize]
public abstract class BasicController : Controller
{
    // Apenas usuários autenticados
}

[AllowAnonymous]
public class HomeController : Controller
{
    // Aceita não autenticados (login)
}
```

#### **Por Action:**
```csharp
[AllowAnonymous]
public IActionResult Index() { }  // Rota de login - sem autenticação

[HttpGet]
public async Task<IActionResult> Index()  // Outras rotas - sempre autenticadas
{
    return await Task.FromResult(View()).ConfigureAwait(false);
}
```

**NÃO há uso de `[Authorize(Roles = "...")]`** - O projeto não utiliza atributos de roles em actions. A autorização é feita através do **constructor de BasicController** que valida se o usuário passou na policy.

### Fluxo de Autorização em BasicController (REAL)

Quando qualquer controller herda de `BasicController`:

```csharp
public BasicController(
    [FromServices] IWebHostEnvironment environment,
    Policy policy,                              // Policy exigida para esta rota
    IHttpContextAccessor context,
    IConfiguration configuration,
    SignInManager<IdentityUser> signInManager,
    UserManager<IdentityUser> userManager,
    IPrincipal principal,
    IUser user,
    IStoreRoles storeRoles
)
{
    IsAuthorized = false;
    Environment = environment;
    Context = context;
    Configuration = configuration;
    SignInManager = signInManager;
    UserManager = userManager;
    UserIdentity = user;
    StoreRoles = storeRoles;
    Principal = principal;
    
    var _rota = context?.HttpContext?.Request?.Path.ToString() ?? "";
    
    try
    {
        // 1. Cria AppServicosGestaoIdentidade para obter credenciais
        using var app = new AppServicosGestaoIdentidade(configuration, user);
        
        // 2. Chama repositório que executa Stored Function SQL
        var _credential = Task.Run(async () => await app.GetCredenciaisUsuario())
            .ConfigureAwait(true)
            .GetAwaiter()
            .GetResult();
        
        if (_credential == null)
            throw new Exception("Erro na tentativa de acessar o sistema");
        
        // 3. Define o nível de acesso exigido para esta rota
        NivelAcessoPermitido = policy;
        
        // 4. Valida se o usuário está no nível exigido
        IsAuthorized = storeRoles.IsInPolicy(policy);
        
        // 5. Popula dados de identidade para usar em toda a requisição
        Identidade = new TransferenciaIdentidadeDTO
        {
            IdVendedorLogado = _credential?.IdVendedorLogado ?? 0,
            IdEmpresaLogado = _credential?.IdEmpresaLogado ?? 0,
            IdUsuarioLogado = _credential?.IdUsuarioLogado ?? 0,
            NmUsuarioLogado = _credential?.NmUsuarioLogado ?? "",
            AutoAgendamento = _credential?.AutoAgendamento ?? 0,
            IsAuthorized = IsAuthorized,
            RotaController = _rota
        };
    }
    catch
    {
        // Se ocorrer erro, cria identidade vazia
        Identidade = new TransferenciaIdentidadeDTO
        {
            AutoAgendamento = 0,
            IdVendedorLogado = 0,
            IdEmpresaLogado = 0,
            IdUsuarioLogado = 0,
            NmUsuarioLogado = "",
            IsAuthorized = false,
            RotaController = _rota
        };
    }
}
```

**Pontos-chave:**
1. **Sempre executa** - Injeção de dependência do ASP.NET Core garante construtor sempre chamado
2. **Recupera credenciais do BD** - Via `AppServicosGestaoIdentidade` que chama `RepositorioGestaoIdentidade`
3. **Valida Policy** - Usa `StoreRoles.IsInPolicy()` (hierárquico, não apenas role)
4. **Popula Identidade** - DTO com dados do usuário fica disponível em toda a action via `this.Identidade`
5. **Sem redirect automático** - O controller fica responsável de checar `IsAuthorized`

### Proteção em Views

```csharp
public override ViewResult View(string viewName, object model)
{
    var _logado = User?.Identity?.IsAuthenticated ?? false;
    var _autorizado = StoreRoles?.IsAuthorized ?? false;
    
    if (!_logado || !_autorizado)
    {
        return base.View("../Home/Index");  // Redireciona para login
    }
    
    return base.View(viewName, model);
}
```

---

## 📊 Fluxo de Requisições (EXEMPLOS REAIS)

### 1. Requisição HTTP (Login Real)

**Arquivo: AcessosController.cs** (Público, sem autenticação)

```
[POST /Acessos/Index]
    ↓
[AcessosController.Index(LoginViewModel requestForm)]
    ↓
1. Busca usuário no Identity DB
   var user = await _userManager.FindByEmailAsync(email);
    ↓
2. Valida senha
   var result = await _signInManager.PasswordSignInAsync(
       user.UserName, password, isPersistent: true, lockoutOnFailure: false);
    ↓
3. Se sucesso, obtém roles do usuário
   var roles = await _userManager.GetRolesAsync(user);
    ↓
4. Adiciona claims (incluindo roles)
   var claims = new List<Claim>
   {
       new Claim(ClaimTypes.Name, user.UserName),
       new Claim(ClaimTypes.Email, user.Email)
   };
   foreach (var role in roles)
   {
       claims.Add(new Claim(ClaimTypes.Role, role));
   }
    ↓
5. Cria ClaimsPrincipal e faz sign-in
   var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
   var principal = new ClaimsPrincipal(identity);
   await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    ↓
6. Retorna JSON com redirect
   return Json(new { 
       success = true, 
       redirectUrl = Url.Action("Index", "Calendario", new { area = "Agenda" }) 
   });
    ↓
[Usuário Autenticado com Cookie ✅]
```

### 2. Requisição HTTP (Rota Protegida - CalendarioController)

```
[GET /Agenda/Calendario/Index]
    ↓
[Cookie validado pelo [Authorize]]
    ↓
1. ASP.NET Core chama constructor de CalendarioController
   public CalendarioController(
       IWebHostEnvironment environment,
       IHttpContextAccessor context,
       IConfiguration configuration,
       SignInManager<IdentityUser> SignInManager,
       UserManager<IdentityUser> UserManager,
       IPrincipal principal,
       IUser user,
       IStoreRoles storeRoles)
       : base(environment, Policy.User, context, ...)
    ↓
2. Executa BasicController constructor
   - Cria AppServicosGestaoIdentidade
   - Chama GetCredenciaisUsuario()
        ↓
   3. AppServicosGestaoIdentidade.GetCredenciaisUsuario()
      - Cria RepositorioGestaoIdentidade
      - Cria ServicosGestaoIdentidade
      - Chama _servicosBase.GetCredenciaisUsuario()
           ↓
      4. ServicosGestaoIdentidade.GetCredenciaisUsuario()
         - Chama _repositorio.GetCredenciaisUsuario()
              ↓
         5. RepositorioGestaoIdentidade.GetCredenciaisUsuario()
            - Executa SQL:
              "SELECT * FROM public.sfn_get_credenciais_usuario('username')"
            - Retorna TransferenciaIdentidadeDTO
    ↓
   - Popula Identidade do usuário (ID, empresa, etc)
   - Valida IsInPolicy(Policy.User) → true
   - Popula this.Identidade
    ↓
6. Executa action CalendarioController.Index()
   - Retorna View()
    ↓
[View renderizada com sucesso ✅]
```

### 3. Requisição Ajax (POST com dados - AlterarAgendamentos real)

```
[POST /Agenda/Calendario/AlterarAgendamentos]
    ↓
1. Controller valida dados
   if (dados == null)
       throw new Exception("Dados do formulário vazio");
    ↓
2. Valida com DataAnnotations
   var context = new ValidationContext(dados, null, null);
   var validationResults = new List<ValidationResult>();
   Validator.TryValidateObject(dados, context, validationResults, true);
    ↓
3. Se válido, cria AppServicos com identidade logada
   using var app = new CalendarioAppServicos(
       base.UserIdentity,      // Usuário do request
       base.Configuration,     // Conexão BD
       base.Identidade);       // Dados do usuário (empresa, ID, etc)
    ↓
4. Chama CreateOrUpdate (herda de BaseAppServicos)
   _ = await app.CreateOrUpdate(dados);
        ↓
   5. CalendarioAppServicos.CreateOrUpdate(dados)
      - Chama _servico.CreateOrUpdate(dados)
           ↓
      6. CalendarioServicos.CreateOrUpdate(dados)  [Domain Service]
         - Chama _repositorio.CreateOrUpdate(dados)
              ↓
         7. CalendarioRepositorio.CreateOrUpdate(dados)
            - Executa INSERT/UPDATE no PostgreSQL
            - Retorna ID do registro criado
         ↓
         - Propaga ErrorRepositorio e MessageError
    ↓
    - Propaga status de erro para AppServicos
    ↓
8. Controller retorna resposta JSON
   return await ResponseJson(ResponseJsonTypes.Success);
    ↓
{
    "jsonTypes": "success",
    "mensagem": "Operação realizada com sucesso",
    "data": null,
    "recordsTotal": null
}
    ↓
[JavaScript processa resposta ✅]
```

---

## 📦 DTOs e Entidades

### ResponseMethodJson

DTO padrão para respostas JSON:

```csharp
public class ResponseMethodJson
{
    public string JsonTypes { get; set; }      // "success" | "error" | "warning"
    public string Mensagem { get; set; }       // Mensagem para usuário
    public object Data { get; set; }           // Dados
    public long? RecordsTotal { get; set; }    // Para paginação
}
```

**Uso em Controllers:**
```csharp
return await ResponseJson(
    ResponseJsonTypes.Success,
    "Agendamento criado com sucesso",
    new { id = novoAgendamento.Id },
    1
);
```

### TransferenciaIdentidadeDTO

Transfere dados do usuário entre camadas:
- **IdVendedorLogado**: ID do vendedor/profissional
- **IdEmpresaLogado**: ID da empresa/clínica
- **IdUsuarioLogado**: ID no banco
- **NmUsuarioLogado**: Nome do usuário
- **AutoAgendamento**: Flag para permitir auto-agendamento
- **IsAuthorized**: Passou na validação de policy?
- **RotaController**: Qual rota foi acessada

---

## 💉 Injeção de Dependências

### Configuração em Program.cs

```csharp
// AbstractFactory Pattern
builder.Services.AddScoped<IUser, AspNetUser>();
builder.Services.AddScoped<IStoreRoles, StoreRoles>();

// Acesso HTTP
builder.Services.AddHttpContextAccessor();

// Principal do usuário (System.Security.Principal)
builder.Services.AddTransient<IPrincipal>(provider => 
    provider.GetService<IHttpContextAccessor>()?.HttpContext?.User
);

// ActionContext para MVC
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
```

### Injeção em BasicController

```csharp
public BasicController(
    [FromServices] IWebHostEnvironment environment,
    Policy policy,
    IHttpContextAccessor context,
    IConfiguration configuration,
    SignInManager<IdentityUser> signInManager,
    UserManager<IdentityUser> userManager,
    IPrincipal principal,
    IUser user,
    IStoreRoles storeRoles
)
```

O ASP.NET Core resolve automaticamente:
- ✅ `IWebHostEnvironment` (ambiente)
- ✅ `IHttpContextAccessor` (contexto HTTP)
- ✅ `IConfiguration` (configurações)
- ✅ `SignInManager<IdentityUser>` (Identity)
- ✅ `UserManager<IdentityUser>` (Identity)
- ✅ `IPrincipal` (registrado acima)
- ✅ `IUser` (AspNetUser)
- ✅ `IStoreRoles` (StoreRoles)

---

## ⚙️ Configuração e Deploy

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;User Id=postgres;Password=senha;Database=Agenda;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

### appsettings.Development.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Information"
    }
  }
}
```

### Database: PostgreSQL

Stored Functions:
```sql
-- Obtém credenciais do usuário
SELECT * FROM public.sfn_get_credenciais_usuario('username');

-- Realiza logout
EXEC acessos.ssp_logoutusuario 'username';
```

---

## 🐳 Docker - Containerização da Aplicação

O **Agenda 2.0** é totalmente containerizado usando Docker e Docker Compose. Esta seção explica como a aplicação é empacotada, distribuída e executada em containers.

### Visão Geral da Estratégia Docker

```
┌─────────────────────────────────────────────────────────────┐
│              IMAGEM DOCKER (Dockerfile)                     │
│                                                             │
│ ┌─────────────────────────────────────────────────────┐   │
│ │ ESTÁGIO 1: Build (Compilação)                       │   │
│ │ FROM mcr.microsoft.com/dotnet/sdk:8.0               │   │
│ │ - Restaura pacotes NuGet                            │   │
│ │ - Compila código em Release                         │   │
│ │ - Publica em /app/publish                           │   │
│ └─────────────────────────────────────────────────────┘   │
│                         ↓                                   │
│ ┌─────────────────────────────────────────────────────┐   │
│ │ ESTÁGIO 2: Runtime (Execução)                       │   │
│ │ FROM mcr.microsoft.com/dotnet/aspnet:8.0            │   │
│ │ - Copia apenas binários compilados                  │   │
│ │ - Expõe porta 8080                                  │   │
│ │ - Executa: dotnet Agenda.dll                        │   │
│ └─────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                             ↓
         CONTAINER (Execução da imagem)
         ├─ Porta 8080 mapeada para host
         ├─ Conecta ao PostgreSQL
         └─ Acessa appsettings.json
```

### Dockerfile (Construção em 2 Estágios)

**Arquivo:** `./Dockerfile`

```dockerfile
# ====================================
# ESTÁGIO 1: Build (Compilação)
# ====================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia arquivo da solução
COPY *.sln .

# Copia arquivos .csproj de CADA camada
COPY src/Agenda.Infra/Agenda.Infra.csproj src/Agenda.Infra/
COPY src/Agenda.Dominio/Agenda.Dominio.csproj src/Agenda.Dominio/
COPY src/Agenda.Repositorio/Agenda.Repositorio.csproj src/Agenda.Repositorio/
COPY src/Agenda.Database/Agenda.Database.sqlproj src/Agenda.Database/
COPY src/Agenda.Aplicacao/Agenda.Aplicacao.csproj src/Agenda.Aplicacao/
COPY src/Agenda/Agenda.csproj src/Agenda/

# Restaura pacotes NuGet da solução completa
RUN dotnet restore "Agenda.sln"

# Copia todo o código-fonte
COPY src/. .

# Define diretório de trabalho para projeto principal
WORKDIR "/src/Agenda"

# Publica a aplicação em Release
# /p:UseAppHost=false permite rodar sem instalação local do runtime
RUN dotnet publish "Agenda.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ====================================
# ESTÁGIO 2: Runtime (Execução)
# ====================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copia APENAS os binários compilados do estágio anterior
# Isso reduz o tamanho da imagem final drasticamente
COPY --from=build /app/publish .

# Expõe a porta 8080 para tráfego externo
EXPOSE 8080

# Comando para iniciar a aplicação
ENTRYPOINT ["dotnet", "Agenda.dll"]
```

**Por que 2 estágios?**

| Aspecto | Estágio 1 (Build) | Estágio 2 (Runtime) |
|---------|------------------|-------------------|
| **Imagem Base** | `dotnet/sdk:8.0` (1.4 GB) | `dotnet/aspnet:8.0` (220 MB) |
| **Responsabilidade** | Compilar código | Executar binários |
| **Tamanho Final** | Descartado | Usado no container |
| **Segurança** | Não precisa estar no container | ✅ Apenas código compilado |

**Resultado:** Imagem final ~300 MB em vez de 1.4 GB

### Docker Compose (Orquestração)

**Arquivo:** `./docker-compose.yml`

```yaml
version: '3.8'

services:
  # ============================================
  # SERVIÇO 1: Aplicação ASP.NET Core
  # ============================================
  agenda-app:
    build:
      context: .                    # Diretório raiz do projeto
      dockerfile: Dockerfile        # Usa o Dockerfile acima
    container_name: agenda-app      # Nome identificável
    restart: always                 # Reinicia se cair
    ports:
      - "8080:8080"                 # Mapeia porta 8080
    extra_hosts:
      # Permite que container acesse o PC como "host.docker.internal"
      # Útil se PostgreSQL está rodando no host
      - "host.docker.internal:host-gateway"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      
      # STRING DE CONEXÃO DO BANCO
      # Host=host.docker.internal aponta para o computador (não para container interno)
      # Port=5434 é a porta onde PostgreSQL está exposto no host
      - ConnectionStrings__DefaultConnection=Host=host.docker.internal;Port=5434;Database=agenda_prod_db;Username=agenda_user;Password=qwas7845@
    
    # Aguarda por outro serviço antes de iniciar (opcional)
    # depends_on:
    #   - postgres
    
    # Logs
    logging:
      driver: "json-file"
      options:
        max-size: "10m"
        max-file: "3"

  # ============================================
  # SERVIÇO 2: PostgreSQL (OPCIONAL)
  # ============================================
  # postgres:
  #   image: postgres:15-alpine
  #   container_name: agenda-postgres
  #   restart: always
  #   ports:
  #     - "5432:5432"
  #   environment:
  #     POSTGRES_DB: agenda_prod_db
  #     POSTGRES_USER: agenda_user
  #     POSTGRES_PASSWORD: qwas7845@
  #   volumes:
  #     # Persiste dados do PostgreSQL
  #     - postgres_data:/var/lib/postgresql/data
  #   healthcheck:
  #     test: ["CMD-SHELL", "pg_isready -U agenda_user -d agenda_prod_db"]
  #     interval: 10s
  #     timeout: 5s
  #     retries: 5

# volumes:
#   postgres_data:
#     driver: local
```

**Explicação:**

- **agenda-app**: Sua aplicação ASP.NET Core
  - `build: .` compila usando o Dockerfile
  - `ports: 8080:8080` expõe a aplicação na porta 8080
  - `extra_hosts` permite acessar `host.docker.internal` (seu PC)
  - `environment` injeta variáveis (string de conexão)

- **postgres** (comentado): PostgreSQL como container
  - Descomentado quando você quer rodar BD dentro do Docker
  - `postgres_data` volume persiste os dados entre container restarts

### Como Usar Docker

#### 1️⃣ **Build da Imagem**

```bash
# Na raiz do projeto (onde está o Dockerfile)
docker build -t agenda:latest .

# Opções úteis:
docker build -t agenda:v1.0.0 .              # Com tag de versão
docker build -t agenda:latest --no-cache .   # Sem cache (rebuild completo)
docker build -t agenda:latest -f Dockerfile .  # Especificar Dockerfile
```

**Resultado:**
```
[+] Building 45.2s (15/15) FINISHED
 => => writing image sha256:abc123def456 0.0s
 => => naming to docker.io/library/agenda:latest 0.0s
```

#### 2️⃣ **Executar via Docker Compose**

```bash
# Na raiz do projeto (onde está docker-compose.yml)
docker-compose up -d

# Opções úteis:
docker-compose up                        # Rodar em foreground (vê logs)
docker-compose up -d                     # Rodar em background
docker-compose up --build                # Rebuild e rodar
docker-compose up -d --force-recreate    # Forçar recreação

# Ver logs
docker-compose logs -f agenda-app        # Follow logs da aplicação
docker-compose logs --tail=50            # Últimas 50 linhas

# Parar
docker-compose down                      # Para todos os containers
docker-compose down -v                   # Para e remove volumes
```

#### 3️⃣ **Executar Container Único**

```bash
# Rodar imagem já built
docker run -d \
  --name agenda-app \
  -p 8080:8080 \
  -e "ConnectionStrings__DefaultConnection=Host=host.docker.internal;Port=5434;Database=agenda_prod_db;Username=agenda_user;Password=qwas7845@" \
  -e "ASPNETCORE_ENVIRONMENT=Production" \
  agenda:latest

# Acessar aplicação
curl http://localhost:8080
```

### Cenários de Uso

#### Cenário 1: PostgreSQL no Host (Atual)

```
┌──────────────────────────────────┐
│      Seu Computador (Host)       │
│                                  │
│  ┌──────────────────────────┐   │
│  │  PostgreSQL (Porta 5434) │   │
│  └──────────────────────────┘   │
│            ↑                    │
│            │ host.docker.internal
│            │                    │
│  ┌──────────────────────────┐   │
│  │  Docker Container        │   │
│  │  agenda-app:8080         │   │
│  └──────────────────────────┘   │
│                                  │
└──────────────────────────────────┘
         ↓
    http://localhost:8080
```

**Vantagens:**
- ✅ Banco não precisa estar no Docker
- ✅ Fácil de debugar banco diretamente
- ✅ Dados persisted no host

**docker-compose.yml simplificado:**
```yaml
services:
  agenda-app:
    build: .
    ports:
      - "8080:8080"
    extra_hosts:
      - "host.docker.internal:host-gateway"
    environment:
      - ConnectionStrings__DefaultConnection=Host=host.docker.internal;Port=5434;Database=agenda_prod_db;Username=agenda_user;Password=qwas7845@
```

#### Cenário 2: PostgreSQL em Container (Produção)

```
┌────────────────────────────────────────────┐
│         Docker Compose Network             │
│                                            │
│  ┌──────────────────┐  ┌────────────────┐ │
│  │  agenda-app      │  │  postgres      │ │
│  │  :8080           │  │  :5432         │ │
│  │ (exposed)        │  │ (internal)     │ │
│  └──────────────────┘  └────────────────┘ │
│         ↑                    ↑             │
│         └────── postgres ────┘             │
│       (network interno)                    │
└────────────────────────────────────────────┘
         ↓
    http://localhost:8080
```

**docker-compose.yml completo:**
```yaml
services:
  agenda-app:
    build: .
    ports:
      - "8080:8080"
    depends_on:
      postgres:
        condition: service_healthy
    environment:
      # Host="postgres" resolve via Docker DNS
      - ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=agenda_prod_db;Username=agenda_user;Password=qwas7845@

  postgres:
    image: postgres:15-alpine
    ports:
      - "5432:5432"
    environment:
      POSTGRES_DB: agenda_prod_db
      POSTGRES_USER: agenda_user
      POSTGRES_PASSWORD: qwas7845@
    volumes:
      - postgres_data:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U agenda_user"]
      interval: 10s
      timeout: 5s
      retries: 5

volumes:
  postgres_data:
```

**Vantagens:**
- ✅ Tudo containerizado (produção)
- ✅ Dados em volume Docker (persistem entre restarts)
- ✅ Sem depender do host
- ✅ Fácil de escalar

### Variáveis de Ambiente em Container

**Mapeamento de appsettings.json → Variáveis Docker:**

| appsettings.json | Variável Docker | Exemplo |
|------------------|-----------------|---------|
| `ConnectionStrings:DefaultConnection` | `ConnectionStrings__DefaultConnection` | `Host=postgres;Port=5432;...` |
| `Logging:LogLevel:Default` | `Logging__LogLevel__Default` | `Information` |
| `AllowedHosts` | `AllowedHosts` | `*` |

**Em docker-compose.yml:**
```yaml
environment:
  - ASPNETCORE_ENVIRONMENT=Production
  - ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=agenda_prod_db;Username=agenda_user;Password=qwas7845@
  - Logging__LogLevel__Default=Information
  - AllowedHosts=*
```

### Troubleshooting Docker

#### Problema: "Connection refused"

```bash
# Container não consegue conectar ao PostgreSQL no host
# Solução: Usar host.docker.internal

docker-compose logs agenda-app  # Ver erro
```

**Checklist:**
- ✅ PostgreSQL está rodando no host? `netstat -an | findstr 5434`
- ✅ String de conexão tem `host.docker.internal`?
- ✅ Porta 5434 está correta?

#### Problema: "Port is already in use"

```bash
# Porta 8080 já está sendo usada
docker ps  # Ver containers rodando
docker stop <container_id>

# Ou usar porta diferente
docker run -p 8081:8080 agenda:latest
```

#### Problema: Imagem muito grande

```bash
# Verificar tamanho da imagem
docker images agenda

# Limpar imagens não usadas
docker image prune -a

# Rebuildar sem cache
docker build --no-cache -t agenda:latest .
```

#### Problema: Container cai imediatamente

```bash
# Ver logs de erro
docker-compose logs agenda-app

# Rodar em foreground para ver saída
docker-compose up agenda-app  # Sem -d

# Debugar container
docker run -it agenda:latest /bin/bash
```

### Deployment em Produção

#### Build e Push para Registry

```bash
# Login no Docker Hub
docker login

# Tagar imagem
docker tag agenda:latest seuusuario/agenda:latest
docker tag agenda:latest seuusuario/agenda:v1.0.0

# Push
docker push seuusuario/agenda:latest
docker push seuusuario/agenda:v1.0.0

# Rodar em outro servidor
docker pull seuusuario/agenda:latest
docker run -d -p 8080:8080 seuusuario/agenda:latest
```

#### Usar em Kubernetes (YAML)

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: agenda-app
spec:
  replicas: 3
  selector:
    matchLabels:
      app: agenda
  template:
    metadata:
      labels:
        app: agenda
    spec:
      containers:
      - name: agenda
        image: seuusuario/agenda:latest
        ports:
        - containerPort: 8080
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: agenda-secrets
              key: connection-string
```

### Resumo de Comandos Docker

```bash
# Build e Compose
docker build -t agenda:latest .
docker-compose up -d
docker-compose logs -f

# Verificar status
docker ps
docker images
docker inspect agenda-app

# Parar e limpar
docker-compose down
docker container prune
docker image prune

# Executar comandos no container
docker exec -it agenda-app bash
docker exec -it agenda-app dotnet --version
```

### Próximas Melhorias

- 🔒 Usar secrets do Docker para senhas
- 📊 Adicionar health checks melhorados
- 🔍 Configurar logging centralizado
- 📈 Implementar auto-scaling no Kubernetes
- 🚀 CI/CD pipeline com GitHub Actions

---

## 💼 Regras de Negócio (REAIS)

### 1. Validação de Identidade

**Em BaseAppServicos (Camada de Aplicação):**
```csharp
public BaseAppServicos(IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
{
    var _rota = "<br />Rota Controller:" + identidade?.RotaController ?? "";
    
    // 1. Valida se identidade foi preenchida
    if (identidade == null)
        throw new Exception("Usuário sem permissão de acesso a está função do sistema: Identidade do Usuário Vazio" + _rota);
    
    // 2. Valida se passou na policy
    if (!(identidade?.IsAuthorized ?? false))
        throw new Exception("Usuário sem permissão de acesso a está função do sistema: Nível de Acesso Negado" + _rota);
    
    // 3. Valida se tem configuração
    if (configuration == null)
        throw new Exception("Usuário sem permissão de acesso a está função do sistema: Configuração de Acesso Vazio" + _rota);
    
    Identidade = identidade;
}
```

**Em BaseRepositorio (Camada de Repositório):**
```csharp
public BaseRepositorio(IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
{
    // Mesmas validações
    if (identidade == null)
        throw new Exception("Usuário sem permissão: Identidade do Usuário Vazio");
    
    if (!(identidade?.IsAuthorized ?? false))
        throw new Exception("Usuário sem permissão: Nível de Acesso Negado");
    
    if (configuration == null)
        throw new Exception("Usuário sem permissão: Configuração de Acesso Vazio");
}
```

**Resultado:** Se identidade é null ou IsAuthorized é false, as camadas de negócio e repositório falham IMEDIATAMENTE.

### 2. Filtragem de Dados por Empresa/Vendedor

**No Repositorio (CalendarioRepositorio):**
```csharp
public async Task<RetornoGridPaginado<Calendario>> CarregarGridEnventosCalendario(
    DataTableSearch search, int start, int draw, int length = 10)
{
    // Filtra SEMPRE por IdVendedorLogado e IdUsuarioLogado da identidade
    var _query = string.Format(
        "SELECT * FROM public.ssp_carregargridagendas ({0}, {1}, {2}, {3}, '{4}');",
        _identidade.IdVendedorLogado,    // ← FILTRO OBRIGATÓRIO
        _identidade.IdUsuarioLogado,     // ← FILTRO OBRIGATÓRIO
        start, length, (search?.value ?? "")?.Trim()
    );
    
    var cn = new SqlSystemConnect(ConnectionString);
    var _result = cn.Query<Calendario>(_query, buffered: true, commandTimeout: 1440);
}
```

**Regra:** NENHUMA query pode ser executada sem passar IdVendedor e IdUsuario

### 3. Tratamento de Erros Padronizado

**Fluxo em BaseServicos (Domínio):**
```csharp
public virtual async Task<long> CreateOrUpdate(TEntity entity)
{
    try
    {
        // Executa repositório
        await _repositorio.CreateOrUpdate(entity);
        
        // Valida erro
        if (_repositorio.ErrorRepositorio)
            throw new Exception(_repositorio.MessageError);
        
        return await Task.FromResult(_repositorio.ID).ConfigureAwait(true);
    }
    catch (Exception ex)
    {
        ID = 0;
        ErrorRepositorio = true;
        MessageError = ex.Message;
        throw new TratamentoExcecao(ex);  // ← Padrão de exceção
    }
}
```

**Fluxo em BaseAppServicos (Aplicação):**
```csharp
public virtual async Task<long> CreateOrUpdate(TEntity entity)
{
    var _return = await _servicosBase.CreateOrUpdate(entity).ConfigureAwait(true);
    
    // Propaga status
    ErrorRepositorio = _servicosBase.ErrorRepositorio;
    MessageError = _servicosBase.MessageError;
    
    return _return;
}
```

**Resultado:** Controller sempre sabe se teve erro via ErrorRepositorio flag

### 4. Validação de Dados

**Em Controller (CalendarioController):**
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<JsonResult> AlterarAgendamentos([FromForm] Calendario dados)
{
    try
    {
        // 1. Valida se nulo
        if (dados == null)
            throw new Exception("Dados do formulário vázio");
        
        // 2. Valida DataAnnotations
        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
        var context = new ValidationContext(dados, null, null);
        var validationResults = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(dados, context, validationResults, true);
        
        if (validationResults.Any())
        {
            var _erroMensagem = validationResults.FirstOrDefault()?.ErrorMessage ?? "Erro";
            throw new TratamentoExcecao(_erroMensagem.Traduzir());
        }
        
        // 3. Se tudo ok, executa operação
        using var app = new CalendarioAppServicos(
            base.UserIdentity, base.Configuration, base.Identidade);
        _ = await app.CreateOrUpdate(dados);
        
        return await ResponseJson(ResponseJsonTypes.Success);
    }
    catch (TratamentoExcecao e) 
    { 
        return await ResponseJson(ResponseJsonTypes.Error, e.Message); 
    }
}
```

**Padrão:**
1. Valida null
2. Valida DataAnnotations
3. Executa operação
4. Trata TratamentoExcecao

### 5. Multi-Tenancy (Empresa Isolada)

**Dados na TransferenciaIdentidadeDTO:**
```csharp
public class TransferenciaIdentidadeDTO
{
    public long IdEmpresaLogado { get; set; }     // ← Empresa do usuário
    public long IdVendedorLogado { get; set; }    // ← Vendedor/Profissional
    public long IdUsuarioLogado { get; set; }     // ← Usuário
    public string NmUsuarioLogado { get; set; }
    public int AutoAgendamento { get; set; }
    public bool IsAuthorized { get; set; }
    public string RotaController { get; set; }
}
```

**Como funciona:**
1. Login → AcessosController.Index()
2. Valida usuário no Identity
3. Chama SQL: `sfn_get_credenciais_usuario('username')`
4. Stored Function retorna IdEmpresa e IdVendedor do usuário
5. TODAS as queries subsequentes filtram por estes IDs

**Resultado:** Usuário só vê dados da sua empresa

### 6. Hierarquia de Acesso em StoreRoles

**Validação em StoreRoles:**
```csharp
public bool IsInPolicy(Policy roleName)
{
    IsAuthorized = false;
    if (Roles == null || !Roles.Any())
        return false;
    
    int userMaxPolicy = 0;
    int requiredPolicy = (int)roleName;
    
    // Encontra o nível máximo do usuário
    foreach (var role in Roles)
    {
        var policy = MapRoleNameToPolicy(role);
        if (policy.HasValue && (int)policy > userMaxPolicy)
            userMaxPolicy = (int)policy.Value;
    }
    
    // Compara: usuário >= requisição
    IsAuthorized = userMaxPolicy >= requiredPolicy;
    return IsAuthorized;
}

private static Policy? MapRoleNameToPolicy(string role)
{
    var normalized = NormalizeString(role);
    
    return normalized switch
    {
        "Usuario" => Policy.User,
        "vendedor" => Policy.Vendedor,
        "enfermeira" => Policy.Enfermeira,
        "gerente" => Policy.Gerente,
        "diretor" => Policy.Diretor,
        "administrador" => Policy.Admin,
        "desenvolvedor" => Policy.Developer,
        _ => null
    };
}
```

**Exemplos:**
- Usuário com role "Admin" (6) acessa Policy.Gerente (4)? ✅ SIM (6 >= 4)
- Usuário com role "Vendedor" (2) acessa Policy.Gerente (4)? ❌ NÃO (2 < 4)
- Usuário com role "Developer" (7) acessa Policy.Admin (6)? ✅ SIM (7 >= 6)

---

## 🔄 Ciclo de Vida de Uma Requisição (Exemplo Completo e Real)

### Criar um novo Agendamento - CalendarioController

```csharp
// ============================================================
// 1. CONTROLLER: CalendarioController.AlterarAgendamentos()
// ============================================================
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<JsonResult> AlterarAgendamentos([FromForm] Calendario dados)
{
    try
    {
        // Valida dados vazios
        if (dados == null)
            throw new Exception("Dados do formulário vazio");
        
        // Valida modelo com DataAnnotations
        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
        var context = new ValidationContext(dados, null, null);
        var validationResults = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(dados, context, validationResults, true);
        
        if (validationResults.Any())
        {
            var erro = validationResults.FirstOrDefault()?.ErrorMessage ?? "Erro";
            throw new TratamentoExcecao(erro.Traduzir());
        }
        
        // ============================================================
        // 2. APLICAÇÃO: Cria CalendarioAppServicos
        // ============================================================
        using var app = new CalendarioAppServicos(
            base.UserIdentity,      // IUser do request (AspNetUser)
            base.Configuration,     // IConfiguration (appsettings)
            base.Identidade);       // TransferenciaIdentidadeDTO 
                                    // {IdVendedor: 5, IdEmpresa: 2, IdUsuario: 1, IsAuthorized: true}
        
        // ============================================================
        // 3. APLICAÇÃO: CalendarioAppServicos.CreateOrUpdate()
        // ============================================================
        // Construtor de CalendarioAppServicos:
        // - Cria CalendarioRepositorio(accessor, config, identidade)
        // - Cria CalendarioServicos(_repositorio, accessor, config, identidade)
        // - SetBaseServicos(_servico)
        
        // Executa CreateOrUpdate (herda de BaseAppServicos)
        _ = await app.CreateOrUpdate(dados);
        
        // ============================================================
        // 4. DOMINIO: CalendarioServicos.CreateOrUpdate()
        // ============================================================
        // Chama: await _servicosBase.CreateOrUpdate(entity);
        // _servicosBase é CalendarioServicos que herda de BaseServicos
        
        public virtual async Task<long> CreateOrUpdate(TEntity entity)
        {
            try
            {
                // Chama repositório
                await _repositorio.CreateOrUpdate(entity);
                
                // Valida erros
                if (_repositorio.ErrorRepositorio)
                    throw new Exception(_repositorio.MessageError);
                
                return await Task.FromResult(_repositorio.ID).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                ErrorRepositorio = true;
                MessageError = ex.Message;
                throw new TratamentoExcecao(ex);
            }
        }
        
        // ============================================================
        // 5. REPOSITÓRIO: CalendarioRepositorio.CreateOrUpdate()
        // ============================================================
        public override async Task<long> CreateOrUpdate(Calendario entity)
        {
            // Acessa PostgreSQL
            // Executa INSERT/UPDATE na tabela
            // Retorna ID do registro
        }
        
        // ============================================================
        // 6. CONTROLLER: Recebe resultado e retorna JSON
        // ============================================================
        return await ResponseJson(ResponseJsonTypes.Success);
        
        // ResponseJson retorna:
        {
            "jsonTypes": "success",
            "mensagem": "Operação realizada com sucesso",
            "data": null,
            "recordsTotal": null
        }
    }
    catch (TratamentoExcecao e) 
    { 
        return await ResponseJson(ResponseJsonTypes.Error, e.Message); 
    }
    catch (Exception ex) 
    { 
        return await ResponseJson(ResponseJsonTypes.Error, ex.Message); 
    }
}
```

### Fluxo Visual Completo:

```
┌─────────────────────────────────────────────────────────────────┐
│ APRESENTAÇÃO: CalendarioController                              │
│ - Recebe dados do formulário                                    │
│ - Valida com DataAnnotations                                    │
│ - Cria AppServicos                                              │
│ - Retorna JSON (ResponseJson)                                   │
└────────────────────────┬────────────────────────────────────────┘
                         │ Injeta: IUser, IConfiguration
                         │ Passa: TransferenciaIdentidadeDTO
                         │
┌────────────────────────▼────────────────────────────────────────┐
│ APLICAÇÃO: CalendarioAppServicos                                │
│ - Orquestra repositório e serviço                               │
│ - Propaga erros (ErrorRepositorio, MessageError)                │
│ - Passa Identidade para camadas inferiores                      │
└────────────────────────┬────────────────────────────────────────┘
                         │ Injeta: ICalendarioServicos
                         │
┌────────────────────────▼────────────────────────────────────────┐
│ DOMÍNIO: CalendarioServicos                                     │
│ - Contém lógica de negócio                                      │
│ - Valida erros de repositório                                   │
│ - Propaga erros para camada acima                               │
└────────────────────────┬────────────────────────────────────────┘
                         │ Injeta: ICalendarioRepositorio
                         │ Identidade valida acesso ao BD
                         │
┌────────────────────────▼────────────────────────────────────────┐
│ REPOSITÓRIO: CalendarioRepositorio                              │
│ - Valida Identidade.IsAuthorized no construtor                  │
│ - Executa queries SQL/Stored Procedures                         │
│ - Filtra dados por IdVendedor e IdUsuario                       │
│ - Mapeia resultado para Calendario<T>                           │
└────────────────────────┬────────────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────────────┐
│ BANCO DE DADOS: PostgreSQL                                      │
│ - INSERT/UPDATE agendamentos                                    │
│ - Executa Stored Procedures                                     │
│ - Retorna dados mapeados                                        │
└─────────────────────────────────────────────────────────────────┘
```

**Fluxo de Erros:**

```
Erro no Repositório → ErrorRepositorio = true, MessageError = "..."
         ↓
Serviço detecta → Lança TratamentoExcecao
         ↓
AppServicos captura → Propaga ErrorRepositorio e MessageError
         ↓
Controller trata → Retorna ResponseJson(Error, mensagem)
         ↓
JavaScript exibe erro ao usuário
```

---

## 🧪 Padrões Utilizados

| Padrão | Uso | Localização |
|--------|-----|-------------|
| **Abstract Factory** | Criar IUser, IStoreRoles | AbstractFactory/ |
| **Dependency Injection** | Resolver dependências | Program.cs, Controllers |
| **Repository** | Abstração BD | Agenda.Repositorio |
| **Service Layer** | Lógica de negócio | Agenda.Dominio.Servicos |
| **DTO** | Transferência de dados | Agenda.Dominio.Entidades |
| **Adapter** | SqlSystemConnect | Agenda.Repositorio.Servicos |
| **Strategy** | Políticas de autorização | StoreRoles |

---

## 📝 Exemplo Real: Criar um novo Controller

Para criar um novo controller que herda de `BasicController` e acessa dados do usuário:

```csharp
using Agenda.Aplicacao.Entidades.Agenda;
using Agenda.Controllers;
using Agenda.Dominio.Entidades.Agenda;
using Agenda.Dominio.Interfaces.Autenticacao;
using Controle_Agenda.Dominio.Interfaces.Autenticacao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;
using static Agenda.Dominio.Enuns.IGroupPolicies;
using static Agenda.Dominio.Enuns.IResponseController;

namespace Agenda.Areas.Agenda.Controllers
{
    [Authorize]          // Obriga autenticação
    [Area("Agenda")]     // Define que está em area
    public class MinhaNovaController : BasicController
    {
        // Constructor: Recebe todas as dependências
        // BasicController valida automaticamente a policy
        public MinhaNovaController(
            [FromServices] IWebHostEnvironment environment,
            IHttpContextAccessor context,
            IConfiguration configuration,
            SignInManager<IdentityUser> SignInManager,
            UserManager<IdentityUser> UserManager,
            IPrincipal principal,
            IUser user,
            IStoreRoles storeRoles)
            // Define policy exigida para TODAS as actions deste controller
            : base(environment, Policy.Vendedor, context, configuration, 
                   SignInManager, UserManager, principal, user, storeRoles) 
        { }
        
        // ===== ACTION 1: Retorna View =====
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Dados do usuário logado sempre disponíveis:
            // this.Identidade.IdVendedorLogado
            // this.Identidade.IdEmpresaLogado
            // this.Identidade.IdUsuarioLogado
            // this.Identidade.NmUsuarioLogado
            // this.Identidade.IsAuthorized
            
            return await Task.FromResult(View()).ConfigureAwait(false);
        }
        
        // ===== ACTION 2: Salvar dados via AJAX =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Salvar([FromForm] MinhaEntidade dados)
        {
            try
            {
                // Valida se dados foram enviados
                if (dados == null)
                    throw new Exception("Dados não foram enviados");
                
                // Cria AppServicos com identidade logada
                // O AppServicos garante que dados sejam filtrados por empresa/vendedor
                using var app = new MinhaAppServicos(
                    base.UserIdentity,      // Usuário autenticado
                    base.Configuration,     // Conexão BD
                    base.Identidade);       // IdEmpresa, IdVendedor, etc
                
                // Executa operação de negócio
                var id = await app.CreateOrUpdate(dados);
                
                // Retorna resposta padronizada
                return await ResponseJson(
                    ResponseJsonTypes.Success,
                    "Registro salvo com sucesso",
                    new { id = id }
                );
            }
            catch (TratamentoExcecao e) 
            { 
                return await ResponseJson(ResponseJsonTypes.Error, e.Message); 
            }
            catch (Exception ex) 
            { 
                return await ResponseJson(ResponseJsonTypes.Error, ex.Message); 
            }
        }
        
        // ===== ACTION 3: Carregar grid com paginação =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> CarregarGrid(
            DataTableSearch search = null, 
            int start = 0, 
            int length = 10, 
            int draw = 0)
        {
            try
            {
                using var app = new MinhaAppServicos(
                    base.UserIdentity, 
                    base.Configuration, 
                    base.Identidade);
                
                // AppServicos herda CarregarGrid ou outro método similar
                var resultado = await app.CarregarGrid(search, start, length, draw);
                
                return await ResponseJson(
                    ResponseJsonTypes.Success,
                    data: resultado.data,
                    recordsTotal: resultado.recordsTotal
                );
            }
            catch (Exception ex) 
            { 
                return await ResponseJson(ResponseJsonTypes.Error, ex.Message); 
            }
        }
    }
}
```

### Estructura de AppServicos correspondente:

```csharp
namespace Agenda.Aplicacao.Entidades
{
    public class MinhaAppServicos 
        : BaseAppServicos<MinhaEntidade>,
          IMinhaAppServicos
    {
        private readonly IMinhaServicos _servico;
        
        public MinhaAppServicos(
            IUser? accessor,
            IConfiguration? configuration,
            TransferenciaIdentidadeDTO identidade)
            : base(accessor, configuration, identidade)
        {
            // Cria repositório (passa identidade)
            IMinhaRepositorio _repositorio = new MinhaRepositorio(
                accessor, configuration, identidade);
            
            // Cria serviço de domínio (passa repositório)
            _servico = new MinhaServicos(
                _repositorio, accessor, configuration, identidade);
            
            // Define qual serviço usar
            SetBaseServicos(_servico);
        }
        
        // Expõe métodos específicos
        public async Task<RetornoGridPaginado<MinhaEntidade>> CarregarGrid(
            DataTableSearch search, int start, int length, int draw)
        {
            return await _servico.CarregarGrid(search, start, length, draw);
        }
    }
}
```

**Pontos-chave:**
1. ✅ Constructor herda de BasicController com Policy obrigatória
2. ✅ `this.Identidade` tem ID empresa, ID vendedor, etc
3. ✅ AppServicos é criado com identidade do usuário
4. ✅ Repositório filtra automaticamente por IdVendedor/IdEmpresa
5. ✅ Erros são propagados via ResponseJson
6. ✅ SEM [Authorize(Roles = "...")] - tudo via Policy e BasicController

---

## 🚀 Próximos Passos

1. ✅ Expandir funcionalidades de agendamento
2. ✅ Implementar sistema de notificações
3. ✅ Adicionar relatórios gerenciais
4. ✅ Integrar com calendários (Google Calendar, Outlook)
5. ✅ Mobile app para clientes

---

## 📞 Suporte e Contribuição

# Para dúvidas sobre a arquitetura, entre em contato, vamos trocar experiências!
## linkedin : https://www.linkedin.com/in/jeferson-sena-ti/
## WhatsApp : https://wa.me/71981859864/

**Último atualizado:** Dezembro 23/12/2025.
