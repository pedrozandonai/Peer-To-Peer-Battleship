# Peer-To-Peer-Battleship

Este repositório contém a implementação de um jogo multiplayer de **Batalha Naval** desenvolvido totalmente em **C#** como parte do segundo trabalho prático da disciplina de **Redes de Computadores I** da Universidade do Vale do Rio dos Sinos. O objetivo do projeto é criar uma aplicação de jogo multiplayer utilizando uma arquitetura **peer-to-peer (P2P)** e o protocolo **TCP**, proporcionando uma experiência de jogo em tempo real entre dois jogadores sem a necessidade de um servidor central.

## Índice

- [Contexto e Objetivo](#contexto-e-objetivo)
- [Principais Funcionalidades](#principais-funcionalidades)
- [Como Jogar](#como-jogar)
  - [Dependências Necessárias](#dependências-necessárias)
  - [Dependências Opcionais](#dependências-opcionais)
  - [Baixe o Jogo](#baixe-o-jogo)
    - [Git](#git)
    - [Baixando e executando o arquivo .exe](#baixando-e-executando-o-arquivo-exe)
- [Sobre o Jogo](#sobre-o-jogo)
  - [Opções do Menu Principal](#opções-do-menu-principal)
  - [Regras do Jogo](#regras-do-jogo)
  - [Configurações do Jogo](#configurações-do-jogo)
- [Bugs Conhecidos](#bugs-conhecidos)
  - [Reconexão entre peers](#reconexão-entre-peers)

## Contexto e Objetivo

A aplicação simula o clássico jogo de **Batalha Naval**, onde dois jogadores competem estrategicamente para afundar os navios do adversário. O diferencial deste projeto é o uso de uma arquitetura P2P, na qual cada jogador atua como um nó da rede, responsável pela troca direta de mensagens e sincronização do estado do jogo.

A especificação do trabalho inclui:

- **Arquitetura P2P:** Comunicação direta entre os jogadores utilizando **sockets TCP**.
- **Protocolo de Comunicação:** Troca de mensagens específicas entre ambos os pares.
- **Lógica de Jogo:** Grades de 10x10 para cada jogador, frota composta por navios de diferentes tamanhos, turnos alternados e regras de vitória.
- **Interface do Usuário:** Tipo de interface textual, permitindo interação intuitiva e visualização clara das grades e status do jogo.

Este repositório também reflete a colaboração entre os alunos da turma, promovendo a integração e testes cruzados entre implementações individuais.

## Principais Funcionalidades

1. **Conexão P2P via TCP:** Os jogadores se conectam diretamente e trocam dados confiáveis utilizando sockets TCP.
2. **Protocolo Customizado:** Um conjunto de mensagens e comandos para sincronização e jogabilidade.
3. **Regras e Dinâmica de Jogo:** Grades personalizáveis, lógica de turnos e feedback em tempo real sobre o status da partida.
4. **Sincronização de Estado:** Garantia de consistência entre os jogadores durante toda a partida.
5. **Interface Interativa:** Feedback visual e informativo para os jogadores, incluindo os tiros realizados e resultados.
6. **Configuração de Usuário:** A possibilidade de ajustar configurações como exibição inicial e tempo de expiração das partidas.

## Como Jogar

### Dependências Necessárias

Para rodar o projeto, é necessário ter o **.NET 9.0** ou superior instalado, pois é a versão na qual ele foi desenvolvido e testado. Não há garantias de compatibilidade entre versões anteriores.

1. [.NET 9.0](https://dotnet.microsoft.com/pt-br/download/dotnet/9.0).

### Dependências Opcionais

Caso queira usar o Git para fazer o download do projeto, instale também a ferramenta:

1. [Git](https://git-scm.com/downloads).

### Baixe o Jogo

Utilize um dos seguintes meios para obter acesso ao jogo.

#### Git

Certifique-se de ter instalado tanto as [Dependências Necessárias](#dependências-necessárias) quanto as [Dependências Opcionais](#dependências-opcionais) antes de executar os seguintes comandos.

```bash
git clone https://github.com/pedrozandonai/Peer-To-Peer-Battleship.git

cd Peer-To-Peer-Battleship/src/PeerToPeerBattleship.ConsoleApp

dotnet run Program.cs
```

#### Baixando e executando o arquivo .exe

Ainda a ser implementado.

## Sobre o Jogo

O jogo possui um menu interativo e pode ser configurado de acordo com as preferências do jogador. O menu inicial permite escolher entre diferentes opções, como iniciar uma nova partida, conectar-se a um outro jogador via P2P, visualizar o status das partidas e ajustar as configurações do jogo.

### Opções do Menu Principal

O menu inicial possui 3 opções, sendo elas:

1. **Jogar;**
   - Redireciona o usuário para o [menu do jogo](#opções-do-menu-do-jogo).
2. **Configurações;**
   - Redireciona o usuário para o menu de configurações da aplicação/jogo.
3. **Sair.**
   - Encerra a execução do programa.

### Opções do Menu do Jogo

1. **Criar uma nova partida;**
   - Solicita ao usuário uma porta disponível na rede para criar o servidor TCP e ficar escutando a porta indicada pelo usuário.
   - Fica aguardando a conexão do outro peer na partida.
2. **Juntar a uma partida existente;**
   - Solicita ao usuário um endereço IP e uma Porta para que ele se conecte à uma partida existente, criada por outro peer.
3. **Reconectar a partida encerrada;**
   - Caso uma partida tenha sido encerrada, por causa da persistência de dados, ao iniciar o jogo novamente essa opção estará disponível ao usuário para se reconectar a uma partida que não foi finalizada (que não há ganhadores).
4. **Voltar ao menu principal**
   - Volta para o [menu principal do jogo](#opções-do-menu-principal).

### Regras do Jogo

- O tabuleiro de cada jogador é uma grade 10x10;

- Cada jogador possui uma frota composta por navios de diferentes tamanhos;

  - Quantidade de navios e seus respectivos tamanhos:
    - 1 porta-aviões (5 células);
    - 1 encouraçado (4 células);
    - 2 cruzadores (3 células cada);
    - 2 destróieres (2 células cada).

- O jogo é jogado por turnos, e cada jogador deve tentar adivinhar a posição dos navios adversários;

- O primeiro jogador a afundar toda a frota do oponente vence a partida.

### Configurações do Jogo

O arquivo de configuração é criado automaticamente ao iniciar o jogo pela primeira vez, podendo ser atualizado conforme desejado.

As configurações sempre são salvas dentro da pasta `Documentos/PeerToPeerBattleShip` do usuário, independente do sistema operacional.

- Exibição Inicial: O jogador pode decidir se deseja visualizar ou não a tela inicial do jogo;

- Tempo de Expiração de Partida: Permite definir de quanto em quanto tempo uma partida salva de forma persistente será expirada e eliminada dos dados do usuário;

- Configurações de Conexão: O jogador pode ajustar parâmetros para gerenciar a reconexão e falhas de comunicação durante a partida.

## Bugs Conhecidos

### Reconexão entre peers

Atualmente, a funcionalidade de reconexão entre peers apresenta falhas. Algumas alterações foram realizadas para corrigir o problema, mas a versão final contará apenas com uma opção experimental de tentar se reconectar a uma partida em caso de falha.
Apesar disso, a qualidade da conexão entre os peers é suficientemente boa para minimizar falhas durante uma partida. No entanto, caso uma desconexão ocorra, será necessário criar uma nova partida para continuar jogando.
