# BayesianDiagnosis — Netica (C API) + Wrapper C# + WPF (.NET 8)

Aplicação educacional desenvolvida em C# e WPF (em .NET 8) para testar uma Rede Bayesiana criada no Netica (arquivo `.neta`), utilizando a API oficial do Netica para C por meio de um wrapper em C# (P/Invoke).

A rede exemplificada foi utilizada como um diagnóstico de desempenho escolar, permitindo inserir evidências (variáveis observadas) e visualizar as probabilidades resultantes das hipóteses diagnósticas.

> Projeto com fins educacionais, desenvolvido para a disciplina de Inteligência Artificial do curso de Ciência da Computação.

---

## Estrutura da solução

- **BayesianDiagnosis.Netica**  
  Biblioteca de classes C# contendo:
  - Implementações do wrapper para a Netica C API (P/Invoke)
  - Cliente/utilitários para carregar a rede, definir evidências e consultar probabilidades

- **BayesianDiagnosis**  
  Aplicação WPF (interface gráfica) que utiliza as classes da biblioteca `BayesianDiagnosis.Netica`.

---

## Requisitos

- Windows x64
- .NET 8 SDK
- Visual Studio 2022 (recomendado)
- Netica C API (x64): `netica.dll`

### Importante (licença): `netica.dll` não está no repositório
Por motivos de licenciamento, este repositório **não inclui** a `netica.dll`.

O usuário deve obter a `netica.dll` por conta própria, através do Netica/Norsys, conforme sua licença.

---

## Arquitetura (x64)

A `netica.dll` utilizada é **x64**. Portanto:

- Compile e execute a aplicação como **x64**
- Evite **Any CPU** / **x86**, pois pode causar erro de carregamento da DLL

---

## Como executar

1. Clone este repositório.
2. Abra a solução no Visual Studio.
3. Selecione a configuração **x64** (Debug ou Release).
4. Compile o projeto **BayesianDiagnosis** (WPF).

### Arquivos necessários no diretório de saída (path fixo)

O projeto carrega a DLL do Netica a partir de um **caminho fixo no diretório de saída** (output) da aplicação.

Antes de executar, copie o arquivo `netica.dll` (**x64**) para a pasta onde o `.exe` é gerado, por exemplo:

- `BayesianDiagnosis\bin\x64\Debug\net8.0-windows\`
- `BayesianDiagnosis\bin\x64\Release\net8.0-windows\`

---

## Observações sobre redistribuição
Este repositório não redistribui binários do Netica.  
Qualquer empacotamento/redistribuição do aplicativo com DLLs do Netica deve respeitar os **termos de licença** aplicáveis.
