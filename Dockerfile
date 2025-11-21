# ----
# ESTÁGIO 1: Build (Construção)
# ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o arquivo .sln (que está na raiz do build)
COPY *.sln .

# Copia os arquivos .csproj de CADA camada
# (Usando os nomes de pasta REAIS do seu disco, dentro da pasta 'src')
COPY src/Agenda.Infra/Agenda.Infra.csproj src/Agenda.Infra/
COPY src/Agenda.Dominio/Agenda.Dominio.csproj src/Agenda.Dominio/
COPY src/Agenda.Repositorio/Agenda.Repositorio.csproj src/Agenda.Repositorio/
COPY src/Agenda.Database/Agenda.Database.sqlproj src/Agenda.Database/
COPY src/Agenda.Aplicacao/Agenda.Aplicacao.csproj src/Agenda.Aplicacao/
COPY src/Agenda/Agenda.csproj src/Agenda/

# Restaura todos os pacotes NuGet da Solução
RUN dotnet restore "Agenda.sln"

# Copia todo o resto do código-fonte para compilar
# Copia o conteúdo da pasta 'src' local para o WORKDIR '/src' do container
COPY src/. .

# Define o diretório de trabalho para o projeto principal (Web)
WORKDIR "/src/Agenda"

# Publica a aplicação, otimizada para Release
RUN dotnet publish "Agenda.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ----
# ESTÁGIO 2: Final (Execução)
# ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expõe a porta 8080
EXPOSE 8080

# Comando final para rodar sua aplicação (Agenda.dll)
ENTRYPOINT ["dotnet", "Agenda.dll"]