# 💰 Sistema de Controle de Gastos Residenciais

Sistema full-stack para controle de gastos residenciais, permitindo o cadastro de pessoas, registro de transações financeiras (receitas e despesas) e consulta de totais consolidados.

## 🛠️ Tecnologias Utilizadas

| Camada | Tecnologia |
|--------|-----------|
| **Backend** | .NET 8 — Web API (C#) |
| **Frontend** | React 18 + TypeScript (Vite) |
| **Banco de Dados** | SQLite (via Entity Framework Core) |
| **ORM** | Entity Framework Core 8 |

## 📋 Funcionalidades

### Cadastro de Pessoas
- Criação de pessoas com nome e idade
- Listagem de todas as pessoas cadastradas
- Exclusão de pessoas (com remoção automática de todas as transações associadas)

### Cadastro de Transações
- Registro de transações com descrição, valor, tipo (receita/despesa) e pessoa associada
- Listagem de todas as transações
- **Regra de negócio**: menores de 18 anos só podem cadastrar despesas

### Consulta de Totais
- Exibição do total de receitas, despesas e saldo por pessoa
- Exibição do total geral (soma de todas as pessoas)

## 🚀 Como Executar

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)

### Backend

```bash
cd backend
dotnet restore
dotnet run
```

O backend estará disponível em `http://localhost:5062`. A documentação da API (Swagger) pode ser acessada em `http://localhost:5062/swagger`.

### Frontend

```bash
cd frontend
npm install
npm run dev
```

O frontend estará disponível em `http://localhost:5173`.

## 📐 Arquitetura

### Backend (Web API)

```
backend/
├── Controllers/          # Controladores da API REST
│   ├── PessoasController.cs
│   ├── TransacoesController.cs
│   └── TotaisController.cs
├── Models/               # Entidades do banco de dados
│   ├── Pessoa.cs
│   └── Transacao.cs
├── Data/                 # Contexto do Entity Framework
│   └── AppDbContext.cs
├── DTOs/                 # Data Transfer Objects
│   ├── PessoaDto.cs
│   ├── TransacaoDto.cs
│   └── TotaisDto.cs
└── Program.cs            # Configuração da aplicação
```

### Frontend (React + TypeScript)

```
frontend/src/
├── components/           # Componentes reutilizáveis
├── pages/                # Páginas da aplicação
│   ├── PessoasPage.tsx
│   ├── TransacoesPage.tsx
│   └── TotaisPage.tsx
├── services/             # Comunicação com a API
│   └── api.ts
├── types/                # Tipos TypeScript
│   └── index.ts
└── App.tsx               # Componente raiz com rotas
```

### Endpoints da API

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/pessoas` | Lista todas as pessoas |
| `POST` | `/api/pessoas` | Cadastra uma nova pessoa |
| `DELETE` | `/api/pessoas/{id}` | Remove uma pessoa e suas transações |
| `GET` | `/api/transacoes` | Lista todas as transações |
| `POST` | `/api/transacoes` | Cadastra uma nova transação |
| `GET` | `/api/totais` | Consulta totais por pessoa e geral |

## 📝 Regras de Negócio

1. **Identificadores únicos**: gerados automaticamente pelo banco de dados
2. **Deleção em cascata**: ao remover uma pessoa, todas as suas transações são excluídas automaticamente
3. **Restrição para menores**: pessoas com idade inferior a 18 anos só podem ter transações do tipo "Despesa"
4. **Validação de pessoa**: ao criar uma transação, o sistema verifica se a pessoa informada existe no cadastro
5. **Persistência**: os dados são armazenados em arquivo SQLite (`gastos.db`), garantindo que persistam após fechar a aplicação
