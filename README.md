# WpfApp – Sistema de Pedidos (WPF + JSON)

Aplicação desktop desenvolvida em **WPF (.NET Framework)** para gerenciamento de **Pessoas, Produtos e Pedidos**, utilizando persistência em arquivos **JSON** e padrão de projeto **MVVM (Model–View–ViewModel)**.

Projeto focado em simplicidade, clareza de código.

---------------------------------------------------
📦 Dependências Necessárias
.NET Framework 4.6+ (ou 4.8)

Newtonsoft.Json (JSON.NET)

Instalação via NuGet:

Install-Package Newtonsoft.Json

--------------------------------------------------
Como executar:

abrir a .sln e rodar

--------------------------------------------------

## 🧱 Padrão de Projeto Utilizado

### MVVM – Model View ViewModel

O projeto segue o padrão **MVVM**, separando responsabilidades:

- **Model**: classes de domínio (dados e regras simples)
- **View**: telas (XAML)
- **ViewModel**: lógica de apresentação, comandos e bindings
- **Services**: acesso a dados (JSON), regras de persistência

Benefícios:
- Código mais organizado
- Facilidade de manutenção
- Separação clara entre UI e regra de negócio
- Facilita testes e evolução do sistema

-------------------------------------------------------------

## 📁 Estrutura do Projeto

```text
WpfApp
│
├── Data
│   ├── pessoas.json
│   ├── produtos.json
│   └── pedidos.json
│
├── Models
│   ├── Pessoa.cs
│   ├── Produto.cs
│   ├── Pedido.cs
│   ├── PedidoItem.cs
│   └── Enums
│       ├── FormaPagamento.cs
│       └── StatusPedido.cs
│
├── Services
│   ├── JsonRepository.cs
│   ├── PessoaService.cs
│   ├── ProdutoService.cs
│   ├── PedidoService.cs
│   └── Paths.cs
│
├── ViewModels
│   ├── BaseViewModel.cs
│   ├── RelayCommand.cs
│   ├── PessoasViewModel.cs
│   ├── ProdutosViewModel.cs
│   └── PedidosViewModel.cs
│
├── Views
│   ├── PessoasView.xaml
│   ├── ProdutosView.xaml
│   └── PedidosView.xaml
│
├── MainWindow.xaml
├── App.xaml
└── README.md

🧩 Descrição das Pastas e Classes
📂 Models



Botões:

Adicionar

Remover

Total calculado automaticamente

🧪 Persistência de Dados

Os dados são armazenados localmente em arquivos JSON:

Data/
├── pessoas.json
├── produtos.json
└── pedidos.json
---------------------------------------------------------

📂 Views (Abas da aplicação)
👤 Aba Pessoas

Função: Gerenciar pessoas

Botões:

Novo: limpa campos para cadastro

Salvar: cria ou atualiza pessoa

Excluir: remove pessoa selecionada

Buscar: filtra por nome ou CPF

Limpar: remove filtros
--------------------------------------------------------
📂 Views (Abas da aplicação)
👤 Aba Pessoas

Função: Gerenciar pessoas

Botões:

Novo: limpa campos para cadastro

Salvar: cria ou atualiza pessoa

Excluir: remove pessoa selecionada

Buscar: filtra por nome ou CPF

Limpar: remove filtros
--------------------------------------------------------


📦 Aba Produtos

Função: Gerenciar produtos

Botões:

Novo: prepara inclusão

Salvar: cria ou atualiza produto

Excluir: remove produto

Buscar: filtra por nome, código e faixa de valor

Limpar: remove filtros

------------------------------------------------------
🧪 Persistência de Dados

Os dados são armazenados localmente em arquivos JSON:

Data/
├── pessoas.json
├── produtos.json
└── pedidos.json


Os arquivos são criados automaticamente caso não existam.
---------------------------------------------------------

Data/
├── pessoas.json
├── produtos.json
└── pedidos.json
Os arquivos são criados automaticamente caso não existam.
---------------------------------------------------------


---------------------------------------------------------
Data/
├── pessoas.json
├── produtos.json
└── pedidos.json
Os arquivos são criados automaticamente caso não existam.

📦 Dependências Necessárias
.NET Framework 4.6+ (ou 4.8)

Newtonsoft.Json (JSON.NET)

Instalação via NuGet:

Install-Package Newtonsoft.Json
--------------------------------------------------------

✅ Status do Projeto

✔ CRUD de Pessoas
✔ CRUD de Produtos
✔ CRUD de Pedidos
✔ Cálculo automático de totais
✔ Persistência em JSON
✔ MVVM mínimo funcional

----------------------------------------------------------
👨‍💻 Observação Final

Este projeto foi desenvolvido com foco em clareza, 
organização e boas práticas básicas, sendo adequado 
para avaliação técnica em vagas de estágio ou desenvolvedor júnior.
# WpfApp
