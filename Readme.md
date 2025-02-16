# Snacktech Worker Payment

Este repositório contém uma aplicação worker .NET que processa mensagens de uma fila do Amazon SQS (Simple Queue Service).  O serviço implementa o padrão Dead Letter Queue (DLQ) para lidar com mensagens que não puderam ser processadas, garantindo a confiabilidade e a integridade do fluxo de mensagens.
Essa aplicação faz parte do serviço de processamentos de pedidos do sistema Snacktech.

## Tecnologias Utilizadas

*   **.NET:** Framework principal para o desenvolvimento do serviço worker.
*   **AWS SDK for .NET:** Biblioteca para interagir com os serviços da AWS, incluindo SQS.
*   **xUnit (ou similar):** Framework de testes unitários.

## Pré-requisitos

*   [.NET SDK](https://dotnet.microsoft.com/en-us/download) instalado.
*   Conta AWS com permissões configuradas para acessar o SQS (incluindo filas FIFO e DLQ).
*   Configuração das variáveis de ambiente com as credenciais da AWS (AWS\_ACCESS\_KEY\_ID e AWS\_SECRET\_ACCESS\_KEY) ou configuração do perfil da AWS.
*   Configuração das váriaveis de ambiente para conexão com o banco de dados de pedidos.

## Como construir o projeto

1.  Clone este repositório:
    ```bash
    git clone <URL_DO_REPOSITORIO>
    ```

2.  Navegue até o diretório do projeto:
    ```bash
    cd <DIRETORIO_DO_PROJETO>
    ```

3.  Restaure os pacotes NuGet:
    ```bash
    dotnet restore
    ```

4.  Construa o projeto:
    ```bash
    dotnet build
    ```

## Como executar os testes unitários

1.  Navegue até o diretório do projeto de testes:
    ```bash
    cd <DIRETORIO_DO_PROJETO_DE_TESTES>
    ```

2.  Execute os testes:
    ```bash
    dotnet test
    ```

## Desenvolvimento

### Estrutura do Código

Todo o código fonte da aplicação fica dentro da pasta `src`. A qual contém a pasta da aplicação `src/WorkerService` e a pasta dos testes "src/WorkerService.Tests".
Adicionamente o repo possui as pastas `k8s` com os arquivos de manifestos kubernetes e a pasta `.github/workflws` contendo os pipelines de CI/CD. 

#### WorkerService

Os componentes principais do sistema estão organizados como:
- Pasta `Data`: Classes de configuração e acesso a persistência de dados.
- Pasta `Handlers`: Classes de operacionalização e lógica de negócio para processar as mensagens e atualizar o pagamentos dos pedidos.
- Pasta `Infrastruture`: Classes que cuidam do acesso ao serviço Amazon SQS.
- Classe `Worker`: Classes que representa a aplicação do tipo BackgroundService.

### WorkerService.Tests

A aplicação conta com 2 tipos de testes:

- Testes unitários: um conjunto de testes unitários construídos para garantir que cada unidade de código esteja operando como esperado dentro de cada respectivo contexto.
- Testes BDD: um conjunto de testes criados usando Gherkin e executados com a lib Specflow.

### CI/CD

Este repositório possui um pipeline configurado para executar o build da aplicação e execução de testes unitários e analise de vulnerabilidades com Sonarqube. 
Como resultado de um PR aprovado para a branch main, é desencadeado o processo de deploy para uma infraestrutura de EKS previamente configurada nas variaveis de ambiente.