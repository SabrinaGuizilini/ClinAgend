# 🏥 ClinAgend

[![Demo](https://img.shields.io/badge/Demo-Online-success)](https://clinagend-npb5.onrender.com/)
![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![Blazor](https://img.shields.io/badge/Blazor-Server-blue)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Database-blue)
![MudBlazor](https://img.shields.io/badge/MudBlazor-UI-green)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ED)
![Multi-Tenant](https://img.shields.io/badge/Multi--Clinic-Supported-orange)

Plataforma web para gerenciamento de agendamentos em clínicas médicas, desenvolvida com **.NET 8** e **Blazor Web App (Server)**.

🔗 Ambiente de demonstração: https://clinagend-npb5.onrender.com/

---

## 📋 Sobre o Projeto

O **ClinAgend** é um sistema multi-clínicas criado para facilitar o gerenciamento de pacientes, médicos, usuários e agendas médicas.

A plataforma permite que cada clínica possua seus próprios usuários, profissionais e pacientes, garantindo isolamento dos dados e controle de acesso baseado em perfis.

---

## 📸 Screenshots

### Login

<img width="960" height="540" alt="login" src="https://github.com/user-attachments/assets/1262a74c-6dd5-416b-80e8-9027df4d988b" />

### Home

<img width="960" height="536" alt="home" src="https://github.com/user-attachments/assets/a99373da-c3f6-4ade-bef5-a2f6a5ad344b" />

### Agenda Médica

<img width="960" height="540" alt="agenda1" src="https://github.com/user-attachments/assets/81f4efda-2777-44ff-bb12-18724725d04d" />
<img width="958" height="540" alt="agenda2" src="https://github.com/user-attachments/assets/c04eda7a-682e-464f-b8d8-1970eec79783" />
<img width="960" height="540" alt="cadastroagendamento" src="https://github.com/user-attachments/assets/33a4f3a9-b7de-4df5-8833-84aa1cb6883d" />

### Listagem de Médicos

<img width="960" height="540" alt="medicos" src="https://github.com/user-attachments/assets/7b60b4eb-0141-49bd-af6b-69cf0eec2bbc" />

---

## 🚀 Tecnologias e Bibliotecas Utilizadas

### Backend

* ASP.NET Core Identity
* Entity Framework Core
* PostgreSQL

### Frontend

* Blazor Web App (Server)
* MudBlazor
* Heron.MudCalendar (https://github.com/danheron/Heron.MudCalendar)
* AutoMapper

---

## 🏗️ Arquitetura da Solution

A solução está organizada em camadas para facilitar manutenção e evolução do sistema.

### ClinAgend.Web

Responsável pela interface do usuário (Frontend Blazor).

### ClinAgend.Core

Contém os serviços e regras de negócio da aplicação.

### ClinAgend.Models

Contém:

* Entidades
* DTOs
* Classes compartilhadas

### ClinAgend.Data

Responsável por:

* DbContext
* Repositories
* Migrations
* Comunicação com o banco de dados

---

## 👥 Tipos de Usuário

### Usuário Padrão

Possui acesso a:

* Cadastro de pacientes
* Cadastro de médicos
* Agenda dos médicos da clínica

---

### Usuário Master

Possui todas as permissões do usuário padrão, além de:

* Cadastro de usuários
* Configurações da clínica

---

### Usuário Médico

Usuário vinculado a um médico cadastrado.

Possui acesso apenas:

* À própria agenda
* Aos seus agendamentos

---

### Super Admin

Usuário global da plataforma.

Não está vinculado a nenhuma clínica e possui acesso a:

* Cadastro de clínicas
* Edição de clínicas
* Exclusão de clínicas

---

## ✨ Funcionalidades

### Autenticação

* Login
* Alteração de senha
* Recuperação de senha ("Esqueci minha senha")
* Controle de acesso baseado em perfis

### Gestão de Usuários

* Cadastro de usuários
* Exclusão de usuários

### Gestão de Médicos

* Cadastro de médicos
* Edição de médicos
* Exclusão de médicos
* Configuração de dias de atendimento, intervalo entre atendimentos e máximo de atendimentos diários

### Gestão de Pacientes

* Cadastro de pacientes
* Edição de pacientes
* Exclusão de pacientes

### Home

Página inicial com resumo dos agendamentos do dia e da semana.

#### Para usuários Padrão e Master

* Total de agendamentos do dia
* Resumo da agenda da clínica

#### Para usuários Médicos

* Resumo da própria agenda

### Agenda Médica

* Calendário interativo estilo Google Agenda
* Visualização diária, semanal e mensal
* Gerenciamento completo dos agendamentos
* Bloqueio de horários

### Confirmação de Consultas

* Envio de mensagem de confirmação via WhatsApp
* Mensagem configurável pela clínica
* Geração automática de link de confirmação de presença
* Confirmação pelo paciente através do link enviado

---

## ⚙️ Configuração do Ambiente

### Pré-requisitos

* .NET SDK 8
* PostgreSQL
* Visual Studio 2022 ou superior

---

## 🔧 Configuração do appsettings.json

Preencha os valores do arquivo `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": ""
  },
  "AdminUser": {
    "Email": "",
    "Password": ""
  },
  "EmailSettings": {
    "Email": "",
    "ApiKey": ""
  },
  "AppSettings": {
    "BaseUrl": ""
  }
}
```

### ConnectionStrings

String de conexão com o banco PostgreSQL.

Exemplo:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=ClinAgend;Username=postgres;Password=senha"
}
```

### AdminUser

Usuário Super Admin criado automaticamente via Seed durante a inicialização da aplicação.

Exemplo:

```json
"AdminUser": {
  "Email": "admin@clinagend.com",
  "Password": "SenhaForte@123"
}
```

### EmailSettings

Configurações da integração com o Brevo para envio de e-mails.

```json
"EmailSettings": {
  "Email": "contato@seudominio.com",
  "ApiKey": "SUA_API_KEY"
}
```

### AppSettings

URL base utilizada para geração de links públicos.

Exemplo:

```json
"AppSettings": {
  "BaseUrl": "https://localhost:7000"
}
```

---

## ▶️ Executando o Projeto

Ao iniciar pela primeira vez:

* O banco será criado automaticamente.
* As migrations serão aplicadas automaticamente.
* O usuário Super Admin será criado utilizando os dados configurados em `AdminUser`.

---

## 🐳 Deploy

O projeto está hospedado na plataforma Render utilizando Docker.

Ambiente online:

https://clinagend-npb5.onrender.com/

---

## 🧪 Usuário para Testes

### Clínica

```text
vidaplena
```

### E-mail

```text
recepcao@vidaplenaclinica.com.br
```

### Senha

```text
Recepcao@123
```

Perfil do usuário:

```text
Master
```
