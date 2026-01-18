# 🖥️ WpfApp – Sistema de Vendas com Cadastro de Pessoas, Produtos e Pedidos

## ▶️ Como Executar:

#### ▶️ Passos para execução

1. Clone o repositório:
   ```bash
   git clone https://github.com/marlonagner/WpfApp.git

Abra o projeto no Visual Studio ou JetBrains Ride

  ## 📦 Dependências

   - .NET Framework  4.8**    
      Pelo Site:
         https://dotnet.microsoft.com/pt-br/download/dotnet-framework/net48
         
  Via Terminal: 
  
    winget install Microsoft.NetFx48


   Newtonsoft.Json**

        https://www.nuget.org/packages/newtonsoft.json/ 
   Via Terminal Powershell em Modo Administrador:
    
    Install-Package Newtonsoft.Json -Version 13.0.5
  
  Restaure as dependências:

      dotnet restore

  O pacote é utilizado para serialização e desserialização dos dados em JSON.
   
### 🔧 Pré-requisitos
- Windows 10 ou superior  

  
- IDE **Visual Studio Community**
  
      https://visualstudio.microsoft.com/pt-br/vs/community/  
  **ou**
- **JetBrains Rider**
   
      https://www.jetbrains.com/pt-br/rider/download/?section=windows

Execute o projeto pela IDE
---


## 📋 Funcionalidades

### 👤 Pessoas
- Cadastro de pessoas
- Edição e exclusão
- Busca por **nome ou CPF**
- Persistência em arquivo `pessoas.json`

### 📦 Produtos
- Cadastro de produtos
- Edição e exclusão
- Filtros por:
  - Nome
  - Código
  - Faixa de valor
- Persistência em arquivo `produtos.json`

### 🧾 Pedidos
- Seleção de pessoa
- Adição de **múltiplos produtos com quantidade**
- Cálculo automático do **valor total** (LINQ)
- Seleção da forma de pagamento
- Finalização do pedido (bloqueia edição)
- Listagem de pedidos salvos
- Busca de pedidos por **nome da pessoa ou Id**
- Persistência em arquivo `pedidos.json`

## 🏗️ Arquitetura do Projeto

<img width="205" height="608" alt="image" src="https://github.com/user-attachments/assets/f84971cb-2080-4c7e-abc4-6dfdcc7d6892" />



## 🛠️ Tecnologias Utilizadas

- C#
- WPF
- .NET Framework 4.8
- Newtonsoft.Json
- XAML

---

## 👨‍💻 Autor

Projeto desenvolvido por **Marlon Agner**.




