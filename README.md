# 🏥 Agenda 2.0 - CRM Para Clínicas

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512bd4?logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/Language-C%23-239120?logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![PostgreSQL](https://img.shields.io/badge/Database-PostgreSQL-336791?logo=postgresql)](https://www.postgresql.org/)
[![Dapper](https://img.shields.io/badge/ORM-Dapper-lightgrey)](https://github.com/DapperLib/Dapper)
[![MVC](https://img.shields.io/badge/Frontend-MVC%20%2F%20Razor-512bd4?logo=dotnet)](https://learn.microsoft.com/aspnet/core/mvc/overview)
[![Identity](https://img.shields.io/badge/Auth-Identity-00599C?logo=microsoft)](https://learn.microsoft.com/aspnet/core/security/authentication/identity)
[![Docker](https://img.shields.io/badge/Container-Docker-2496ED?logo=docker)](https://www.docker.com/)

## Sistema completo de CRM e gestão de agendamentos para clínicas. Desenvolvido com foco em **escalabilidade, manutenibilidade e separação de responsabilidades**, utilizando Clean Architecture (Arquitetura em 4 Camadas) e preparado para o padrão Multi-Tenant.

## ✨ Principais Funcionalidades

* 🔐 **Autenticação Segura:** Baseada em ASP.NET Core Identity com cookies criptografados.
* 🛡️ **Autorização Hierárquica:** Sistema de *Policies* em 7 níveis (User ➡️ Vendedor ➡️ Enfermeira ➡️ Gerente ➡️ Diretor ➡️ Admin ➡️ Developer). Permissões herdadas nativamente.
* 🏢 **Multi-Tenancy Integrado:** Isolamento total de dados por Empresa e Vendedor direto nas consultas ao banco.
* 📦 **Totalmente Containerizado:** Pronto para produção com Docker e Docker Compose (Build em 2 estágios).
* ⚡ **Alta Performance:** Uso estratégico do Dapper para consultas complexas (Stored Functions) e Entity Framework para Identity.

---

## 🏛️ Arquitetura do Projeto

### O projeto segue os princípios do **SOLID** e **Clean Architecture**, dividindo a aplicação em responsabilidades claras para facilitar testes e futuras manutenções ou migrações (ex: extração de microsserviços).

```mermaid
graph TD
    subgraph Apresentacao [📱 Apresentação]
        A[Agenda MVC <br/> Controllers, Views, Identity]
    end
    
    subgraph Aplicacao [⚙️ Aplicação]
        B[Agenda.Aplicacao <br/> AppServiços, Orquestração, DTOs]
    end
    
    subgraph Dominio [🧠 Domínio]
        C[Agenda.Dominio <br/> Regras de Negócio, Interfaces, Enums]
    end
    
    subgraph Infraestrutura [💾 Infraestrutura]
        D[Agenda.Repositorio <br/> Dapper, Mapeamento BD]
    end
    
    DB[(PostgreSQL <br/> Stored Procedures)]

    A -->|Injeção de Dependência| B
    B -->|Interfaces| C
    C -->|Interfaces| D
    D --> DB

```

## 🔄 Fluxo de uma Requisição
### Como as camadas interagem na prática (Exemplo: Salvar um agendamento):

```mermaid
sequenceDiagram
    actor Cliente
    participant C as Controller (Apresentação)
    participant AS as AppServiços (Aplicação)
    participant S as Serviços (Domínio)
    participant R as Repositório (Infra)
    participant DB as Banco de Dados
    
    Cliente->>C: POST /AlterarAgendamento
    C->>C: Valida DataAnnotations
    C->>AS: CreateOrUpdate(Identidade, Dados)
    AS->>S: CreateOrUpdate(Dados)
    S->>S: Valida Regras de Negócio
    S->>R: Executa Comando SQL
    R->>DB: INSERT / UPDATE
    DB-->>R: Retorna ID
    R-->>C: Propaga Sucesso/Erro
    C-->>Cliente: JSON Response (Success)

```
## 🛡️ Segurança e Fluxo de Acesso
### A aplicação não confia apenas em Roles estáticas. O acesso é construído através de uma injeção de dependência na classe base (BasicController), garantindo que nenhuma query seja executada sem o escopo da clínica e do usuário logado.

```mermaid

flowchart LR
    A[Login Request] --> B{Identity Valida Senha}
    B -- Falha --> C[Acesso Negado]
    B -- Sucesso --> D[Stored Function busca Credenciais]
    D --> E[Monta TransferenciaIdentidadeDTO]
    E --> F[Valida Policy Hierárquica]
    F -- Nível Insuficiente --> C
    F -- Nível Adequado --> G[Acesso Liberado para Controller]
    
    style C fill:#ff4d4d,stroke:#333,color:#fff
    style G fill:#4CAF50,stroke:#333,color:#fff

```

## 🐳 Como Rodar o Projeto (Docker)
### O projeto está configurado com docker-compose para subir tanto a aplicação (via build de 2 estágios para imagens leves) quanto o banco de dados.

### 1. Requisitos
Docker e Docker Compose instalados.

### 2. Subindo a Aplicação
Na raiz do repositório, execute:

<pre><code>
docker-compose up -d --build
</code></pre>

### Faz o build da imagem (.NET 8) e sobe os containers em background

    
### 3. Acessando
Aplicação: http://localhost:8080

### Banco de Dados (Interno): Porta 5432

## (Nota: Para rodar apontando para um PostgreSQL local na sua máquina, utilize a string de conexão referenciando host.docker.internal no docker-compose.yml).

👨‍💻 Autor
Jeferson Pimentel Sena Full Stack Engineer

💼 [LinkedIn](https://www.linkedin.com/in/jefersonsena-csharp-dotnet/)

📱 [WhatsApp](https://wa.me/71981859864/)


***

Ficou no ponto! Se você quiser que eu adicione mais alguma tecnologia específica q
