# ScanMyBill - Leitor de QR Code para PDF e Imagens

![Plataforma](https://img.shields.io/badge/plataforma-Android%20%7C%20iOS%20%7C%20macOS%20%7C%20Windows-brightgreen)
![Framework](https://img.shields.io/badge/framework-.NET%20MAUI%2010-blue)
![Licença](https://img.shields.io/badge/licença-MIT-green)
![Status](https://img.shields.io/badge/status-Ativo-success)

## Visão Geral

**ScanMyBill** é uma aplicação móvel e desktop multiplataforma que permite aos usuários escanear e extrair códigos QR de documentos PDF e arquivos de imagem (JPG, PNG). A aplicação oferece uma experiência perfeita para gerenciar documentos digitais, detectando automaticamente códigos QR, extraindo seus dados e mantendo um histórico pesquisável e persistente de todos os escaneamentos com recursos avançados de filtragem.

Projetado com arquitetura MVVM moderna e construído no framework .NET MAUI, o ScanMyBill oferece uma experiência nativa em múltiplas plataformas, incluindo Android, iOS, macOS Catalyst e Windows 10/11. A interface intuitiva apresenta UI em português brasileiro com dois fluxos de trabalho principais: escaneamento rápido na aba principal e gerenciamento abrangente do histórico na aba de histórico.

## Recursos

- ✅ **Escaneamento de QR Code Multi-Formato**: Scan de códigos QR diretamente de:
  - Documentos PDF (conversão automática para imagens)
  - Imagens JPEG
  - Imagens PNG
  
- ✅ **Detecção Inteligente de QR Code**: Utiliza biblioteca ZXing para reconhecimento preciso e de alto desempenho

- ✅ **Histórico Persistente**: 
  - Armazenamento automático de todos os registros de scan em banco de dados SQLite local
  - Metadados incluem nome do arquivo, formato do arquivo, timestamp de extração e valor do QR code
  - Registros históricos ilimitados com recuperação rápida

- ✅ **Pesquisa e Filtragem Avançadas**:
  - Pesquisa por nome de arquivo com correspondência de texto parcial
  - Filtrar por formato de arquivo (Tudo, PDF, Imagens)
  - Filtragem combinada de imagens (PNG + JPG como categoria única)
  - Resultados em tempo real (máx. 100 registros por consulta)

- ✅ **Integração com Área de Transferência**: Cópia com um toque dos valores de QR code extraídos para a área de transferência do dispositivo

- ✅ **Acesso Rápido a Scans Recentes**: A aba principal exibe os 5 escaneamentos mais recentes para referência rápida

- ✅ **Suporte Multiplataforma**:
  - Android 31+ (alvo principal)
  - iOS 15+
  - macOS Catalyst 15+
  - Windows 10 (Build 17763) e Windows 11

- ✅ **Timestamps Sensíveis ao Fuso Horário**: Todos os horários de scan armazenados em UTC com conversão automática para fuso horário local para exibição

---

## Pilha Tecnológica

| Componente | Tecnologia | Versão | Propósito |
|-----------|-----------|---------|---------|
| **Framework** | .NET MAUI | 10.0.41 | Framework de UI multiplataforma |
| **Runtime .NET** | .NET | 10.0 | Ambiente de execução |
| **Escaneamento de QR Code** | ZXing.Net.Bindings.SkiaSharp | 0.16.22 | Detecção e decodificação de QR code |
| **Processamento de PDF** | PDFtoImage | 5.2.0 | Converter PDF para imagens bitmap |
| **Banco de Dados** | SQLite3 | 3.x | Persistência de dados local |
| **Gráficos** | SkiaSharp | (implícito) | Gráficos acelerados por hardware |
| **Kit de Ferramentas MVVM** | CommunityToolkit.Mvvm | 8.4.0 | ObservableObject, RelayCommand |
| **Controles de UI** | CommunityToolkit.Maui | 14.0.1 | Comportamentos de layout, conversores |
| **Contêiner DI** | Microsoft.Extensions.DependencyInjection | 10.0.0 | Injeção de dependência |
| **Logging** | Microsoft.Extensions.Logging.Debug | 10.0.0 | Saída de debug |

---

## Arquitetura

### Padrão MVVM

O ScanMyBill segue o padrão arquitetural **Model-View-ViewModel (MVVM)** com clara separação de responsabilidades:

```
┌─────────────────────────────────────────────────────┐
│ Camada de UI (Views)                                │
│ - TabScanPage.xaml                                  │
│ - TabHistoryPage.xaml                               │
└──────────────────┬──────────────────────────────────┘
                   │ Bindings & Comandos
┌──────────────────▼──────────────────────────────────┐
│ Camada de Apresentação (ViewModels)                 │
│ - TabScanPageViewModel                              │
│ - TabHistoryPageViewModel                           │
└──────────────────┬──────────────────────────────────┘
                   │ Chamadas de Serviço
┌──────────────────▼──────────────────────────────────┐
│ Lógica de Negócio & Serviços                        │
│ - QrCodeService           (Operações de Scan)       │
│ - PdfToImageService       (Conversão de PDF)        │
│ - FileChooseService       (Seleção de Arquivo)      │
│ - AlertService            (Diálogos do Usuário)     │
│ - AppNavigationService    (Navegação de Abas)       │
└──────────────────┬──────────────────────────────────┘
                   │ Padrão de Repository
┌──────────────────▼──────────────────────────────────┐
│ Camada de Acesso a Dados                            │
│ - IHistoryRepository / HistoryRepository            │
│ - DatabaseHelper (Conexão SQLite)                   │
└──────────────────┬──────────────────────────────────┘
                   │ Consultas SQL
┌──────────────────▼──────────────────────────────────┐
│ Armazenamento de Dados (Banco de Dados SQLite)      │
│ - Tabela History (Registros de scan de QR code)     │
└─────────────────────────────────────────────────────┘
```

### Componentes-Chave da Arquitetura

**Views (XAML)**
- UI declarativa usando markup XAML
- Vinculação de dados a ViewModels com suporte a vinculação TwoWay
- CollectionView para renderização eficiente de lista
- Componentes Border para estilo moderno

**ViewModels (C#)**
- Propriedades observáveis usando atributo `ObservableProperty`
- Comandos de retransmissão usando `RelayCommand` para ações do usuário
- Coleção HistoryItems sincronizada com repositório
- Gerenciamento de estado de seleção

**Serviços (Camada de Abstração)**
- Interfaces independentes de plataforma (IFileChoose, IPdf, IQrCode, IAlert, IAppNavigation)
- Implementações específicas de plataforma para Android, iOS, macOS, Windows
- Injeção de dependência habilita testes e alternância de plataforma

**Padrão de Repository**
- HistoryRepository abstrai operações SQLite
- Contrato IHistoryRepository habilita alternância flexível de fonte de dados
- Consultas LINQ-to-Objects com filtragem e pesquisa

**Injeção de Dependência (DI)**
- Contêiner centralizado em `MauiProgram.cs`
- Registrado como singletons (instâncias compartilhadas) para serviços
- Registrado como transientes para ViewModels (novas instâncias por requisição)

---

## Pré-requisitos

Antes de compilar o ScanMyBill, verifique se você tem o seguinte instalado:

- **Sistema Operacional**: Windows 10/11, macOS 12+ ou Linux (Ubuntu 22.04+)
- **.NET 10.0 SDK** ou superior [(Download)](https://dotnet.microsoft.com/download/dotnet/10.0)
- **Git** [(Download)](https://git-scm.com)
- **Requisitos Específicos de Plataforma**:
  - **Android**: Android SDK 31+ (API nível 31 ou superior)
  - **iOS**: Xcode 15+ com iOS 15+ como alvo de implantação
  - **macOS Catalyst**: macOS 12+ com Xcode Command Line Tools
  - **Windows**: Windows App SDK Runtime 1.4+

### Verificar Instalação

```bash
dotnet --version
# Deve exibir .NET 10.0.x ou superior
```

---

## Primeiros Passos

### 1. Clonar o Repositório

```bash
git clone https://github.com/diogosapessoa/scan-my-bill.git
cd scan-my-bill
```

### 2. Navegue até o Projeto

```bash
cd ScanMyBill
```

### 3. Restaurar Dependências

```bash
dotnet restore
```

---

## Compilar e Executar

### Android (Plataforma Principal)

**Compilar APK de Debug:**
```bash
dotnet build -f net10.0-android -c Debug
```

**Compilar APK de Release:**
```bash
dotnet publish -f net10.0-android -c Release /p:AndroidKeyStore=true /p:AndroidSigningKeyStore="C:/Users/diogo/AppData/Local/Xamarin/Mono for Android/Keystore/scanmybill/scanmybill.keystore" /p:AndroidSigningKeyAlias=scanmybill /p:AndroidSigningKeyPass=***** /p:AndroidSigningStorePass=*****
```

**Executar em Dispositivo Conectado/Emulador:**
```bash
dotnet run -f net10.0-android
```

### iOS

**Compilar para Simulador:**
```bash
dotnet build -f net10.0-ios -c Debug
```

**Compilar para Dispositivo:**
```bash
dotnet publish -f net10.0-ios -c Release
```

### macOS Catalyst

**Compilar Aplicação:**
```bash
dotnet build -f net10.0-maccatalyst -c Debug
```

**Executar:**
```bash
dotnet run -f net10.0-maccatalyst
```

### Windows 10/11

**Compilar Aplicação:**
```bash
dotnet build -f net10.0-windows10.0.19041.0 -c Debug
```

**Executar:**
```bash
dotnet run -f net10.0-windows10.0.19041.0
```

---

## Guia de Uso

### Escaneando Códigos QR (Aba Escanear)

1. **Abra a Aba Escanear**: Navegue até a aba "Escanear" na navegação inferior
   
2. **Selecionar Tipo de Arquivo**: Escolha um dos três botões de tipo de arquivo:
   - **PDF**: Escanear códigos QR de documentos PDF
   - **JPG**: Escanear códigos QR de imagens JPEG
   - **PNG**: Escanear códigos QR de imagens PNG

3. **Selecionar Arquivo**: O seletor de arquivo nativo abre com filtros apropriados para a plataforma:
   - **Android**: Filtragem de tipo MIME (application/pdf, image/jpeg, image/png)
   - **iOS/macOS**: Filtragem UTType
   - **Windows**: Filtragem de extensão de arquivo (.pdf, .jpg, .jpeg, .png)

4. **Detecção Automática de QR Code**: 
   - Para PDFs: Converte automaticamente para imagem(s) e escaneia
   - Para Imagens: Escaneia diretamente códigos QR
   - Exibe valor extraído em diálogo de confirmação

5. **Copiar para Área de Transferência**: Toque em "Sim" no diálogo de confirmação para copiar o valor do QR para a área de transferência do dispositivo

6. **Ver Scans Recentes**: A seção "Recentes" exibe seus 5 scans mais recentes com acesso de cópia com um toque

### Navegando no Histórico (Aba Histórico)

1. **Abra a Aba Histórico**: Navegue até a aba "Histórico"

2. **Pesquisar por Nome de Arquivo**:
   - Digite na barra de pesquisa para filtrar registros por nome de arquivo
   - Resultados atualizam em tempo real conforme você digita
   - Pressione Enter ou a tecla Pesquisar para executar a pesquisa

3. **Filtrar por Formato**:
   - **Tudo**: Mostrar todos os itens escaneados
   - **PDF**: Mostrar apenas scans originários de PDF
   - **Imagem**: Mostrar scans originários de JPG e PNG
   - Filtros de formato múltiplo podem ser combinados

4. **Interagir com Histórico**:
   - **Tocar em Item**: Copiar o valor do QR para a área de transferência
   - **Deslizar para Esquerda** (dependente de plataforma): Excluir o registro
   - **Menu de Contexto** (se disponível): Ações adicionais

5. **Ver Detalhes**: Cada item de histórico exibe:
   - Ícone de formato de arquivo
   - Nome do arquivo original
   - Timestamp de scan (no fuso horário local)

---

## Estrutura do Projeto

```
ScanMyBill/
│
├── Views/                              # Páginas XAML de UI
│   ├── TabScanPage.xaml               # Interface de escaneamento QR
│   ├── TabScanPage.xaml.cs            # Code-behind
│   ├── TabHistoryPage.xaml            # Interface de navegação de histórico
│   └── TabHistoryPage.xaml.cs         # Code-behind
│
├── ViewModels/                         # ViewModels MVVM
│   ├── TabScanPageViewModel.cs        # Lógica e comandos da aba de scan
│   └── TabHistoryPageViewModel.cs     # Lógica e comandos da aba de histórico
│
├── Services/                           # Lógica de Negócio & Abstração
│   ├── QrCodeService.cs               # Wrapper de escaneamento QR do ZXing
│   ├── PdfToImageService.cs           # Conversão PDF→Bitmap
│   ├── FileChooseService.cs           # Seleção de arquivo específica da plataforma
│   ├── AlertService.cs                # Exibição de diálogos do usuário
│   └── AppNavigationService.cs        # Navegação de shell baseada em abas
│
├── Interfaces/                         # Contratos de Serviço
│   ├── IQrCode.cs                     # Contrato de escaneamento QR
│   ├── IPdf.cs                        # Contrato de conversão PDF
│   ├── IFileChoose.cs                 # Contrato de seletor de arquivo
│   ├── IAlert.cs                      # Contrato de diálogo
│   └── IAppNavigation.cs              # Contrato de navegação
│
├── Repositories/                       # Camada de Acesso a Dados
│   ├── IHistoryRepository.cs          # Contrato de repositório
│   └── HistoryRepository.cs           # Implementação SQLite
│
├── Entities/                           # Modelos de Domínio
│   └── History.cs                     # Registro de scan QR
│
├── Enums/                              # Enumerações
│   └── EFileFormat.cs                 # Tipos de formato de arquivo
│
├── Data/                               # Configuração de Banco de Dados
│   └── DatabaseHelper.cs              # Conexão SQLite e tabelas
│
├── Models/                             # Data Transfer Objects
│   └── FileChooseResult.cs            # DTO de resultado do seletor de arquivo
│
├── Converters/                         # Conversores de Valor XAML
│   └── UtcToLocalConverter.cs         # Formatação DateTime UTC→Local
│
├── Resources/                          # Recursos da App
│   ├── Styles/
│   │   ├── Colors.xaml               # Definições de cor
│   │   └── Styles.xaml               # Estilo de controle
│   ├── Images/                        # Ícones e gráficos de UI
│   ├── Fonts/
│   │   └── Inter.ttf                 # Tipografia Inter
│   ├── AppIcon/                       # Ícones de app para lojas
│   ├── Splash/                        # Imagem de tela de splash
│   └── Raw/
│       └── AboutAssets.txt           # Documentação de ativos
│
├── Platforms/                          # Código Específico de Plataforma
│   ├── Android/
│   │   ├── AndroidManifest.xml       # Manifesto Android
│   │   ├── MainActivity.cs            # Ponto de entrada Android
│   │   ├── MainApplication.cs         # Classe de app Android
│   │   └── Resources/
│   │       └── values/
│   │           └── colors.xml         # Paleta de cores Android
│   ├── iOS/
│   │   ├── AppDelegate.cs             # Delegado de app iOS
│   │   ├── Program.cs                 # Ponto de entrada iOS
│   │   ├── Info.plist                 # Configuração iOS
│   │   └── Resources/
│   ├── MacCatalyst/
│   │   ├── AppDelegate.cs
│   │   ├── Program.cs
│   │   └── Info.plist
│   └── Windows/
│       ├── App.xaml                   # Definição de app Windows
│       ├── App.xaml.cs
│       ├── app.manifest               # Manifesto de app Windows
│       └── Package.appxmanifest       # Configuração de pacote Windows
│
├── Properties/
│   └── launchSettings.json            # Configuração de inicialização
│
├── MauiProgram.cs                     # Contêiner DI & configuração de app
├── App.xaml                           # Recursos de nível de app
├── App.xaml.cs                        # Classe de app
├── AppShell.xaml                      # Shell (layout de navegação de abas)
├── AppShell.xaml.cs                   # Code-behind do shell
├── ScanMyBill.csproj                  # Arquivo de projeto
└── bin/ & obj/                        # Saída de compilação (gerado)
```

### Descrições de Diretórios

| Pasta | Propósito |
|--------|---------|
| **Views** | Páginas XAML e seu code-behind; define layout e controles de UI |
| **ViewModels** | Classes MVVM ViewModel com propriedades observáveis e comandos de retransmissão |
| **Services** | Camada de lógica de negócio; abstrações para preocupações transversais |
| **Repositories** | Camada de acesso a dados; abstrai operações de banco de dados |
| **Entities** | Modelos de domínio representando objetos de negócio principais |
| **Resources** | Recursos estáticos: cores, estilos, imagens, fontes |
| **Platforms** | Arquivos de implementação e configuração específicos de plataforma |

---

## Notas Importantes

### Armazenamento de Banco de Dados

- **Localização**: O arquivo de banco de dados é armazenado no diretório de dados local da aplicação específico da plataforma:
  - **Android**: `/data/data/com.diogosapessoa.scanmybill/files/DATABASE.db`
  - **iOS**: `<App Sandbox>/Library/Local Database/DATABASE.db`
  - **macOS**: `~/Library/Application Support/com.diogosapessoa.scanmybill/DATABASE.db`
  - **Windows**: `%APPDATA%\com.diogosapessoa.scanmybill\DATABASE.db`

- **Backup Automático**: Considere implementar sincronização iCloud/OneDrive para acesso entre dispositivos (roadmap de recursos)

### Filtragem de Arquivo Específica de Plataforma

Cada plataforma usa APIs de seleção de arquivo nativas com filtragem otimizada:

- **Android**: Filtragem baseada em tipo MIME na intenção ACTION_GET_CONTENT
- **iOS/macOS**: Conformidade com identificadores de tipo uniforme (UTType)
- **Windows**: Filtragem baseada em extensão de arquivo via OpenFileDialog

### Considerações de Desempenho

- **Conversão de PDF**: PDFs grandes (>10MB) podem levar 2-5 segundos por conversão de página
- **Detecção de QR**: Normalmente completa em <500ms para resoluções padrão
- **Consultas de Histórico**: Consultas de banco de dados otimizadas para máx. 100 resultados; paginação recomendada para escalonamento futuro
- **Memória**: Bitmaps SkiaSharp são descartados após escaneamento para evitar vazamentos de memória

### Configuração

- **Geração de Fonte XAML**: Habilitada para otimização de desempenho em tempo de compilação
- **Blocos Inseguros**: Habilitados para compatibilidade de plataforma Windows
- **Tipos de Referência Anulável**: Habilitados para garantir segurança nula em toda a base de código
- **Frameworks de Destino**: Net10.0 com direcionar específico de plataforma para recursos ideais

---

## Solução de Problemas

### Problemas Comuns de Compilação

**Problema**: `dotnet build` falha no Android com "Android SDK não encontrado"
```
Solução: 
1. Instale Android SDK 31+ via Android Studio ou sdkmanager
2. Defina a variável de ambiente ANDROID_HOME
   - Windows: Set-Item env:ANDROID_HOME "C:\Android\sdk"
   - macOS/Linux: export ANDROID_HOME=$HOME/Library/Android/sdk
3. Tente compilar novamente
```

**Problema**: `dotnet publish` falha com "Nenhum perfil de provisionamento" no iOS
```
Solução:
1. Abra no Xcode: open ScanMyBill.csproj
2. Selecione Team em Signing & Capabilities
3. Tente novamente na linha de comando
```

**Problema**: Compilação do Windows falha com "Windows App SDK Runtime não instalado"
```
Solução:
1. Baixe Windows App SDK 1.4+ da Microsoft
2. Instale usando o instalador ou gerenciador de pacotes NuGet
3. Tente compilar novamente
```

### Problemas de Tempo de Execução

**Problema**: Código QR não detectado na imagem
```
Solução:
1. Certifique-se de que a imagem está clara e bem iluminada
2. Tente girar ou ampliar a imagem
3. Verifique se o código QR não é muito pequeno (recomenda-se >50x50 pixels)
4. Verifique se o formato de imagem é suportado (JPG, PNG)
```

**Problema**: Conversão de PDF leva muito tempo
```
Solução:
1. Melhore o desempenho reduzindo o tamanho do arquivo PDF
2. Converta e comprima o PDF antes de escanear
3. Tente escanear páginas individuais em vez do documento completo
```

**Problema**: App trava na inicialização
```
Solução:
1. Limpe dados/cache do app
2. Desinstale e reinstale
3. Verifique armazenamento do dispositivo (certifique-se >100MB livres)
4. Revise logs do sistema: `adb logcat` (Android)
```

### Problemas de Banco de Dados

**Problema**: Histórico não persiste após reinicialização
```
Solução:
1. Verifique se as permissões de escrita foram concedidas
2. Verifique disponibilidade de armazenamento do dispositivo
3. Exclua manualmente DATABASE.db e reinicie (será reinicializado)
4. Revise logs da aplicação
```

---

## Contribuindo

Contribuições são bem-vindas! Por favor, siga estas diretrizes:

1. Faça fork do repositório
2. Crie um branch de recurso: `git checkout -b feature/seu-nome-recurso`
3. Confirme as mudanças: `git commit -m "Adicione seu recurso"`
4. Envie para o branch: `git push origin feature/seu-nome-recurso`
5. Abra um Pull Request

---

## Licença

Este projeto está licenciado sob a **Licença MIT** - veja o arquivo LICENSE para detalhes.

---

## Suporte e Contato

Para problemas, sugestões ou questões:
- Abra uma issue no GitHub
- Contato: [Suas Informações de Contato]

---

## Agradecimentos

- **ZXing**: Biblioteca de detecção de QR code
- **PDFtoImage**: Biblioteca de renderização de PDF
- **.NET MAUI**: Framework multiplataforma
- **CommunityToolkit.Mvvm**: Suporte ao padrão MVVM