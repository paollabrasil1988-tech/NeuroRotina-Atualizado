# NeuroRotina – o que mudou nesta versão

## 1. "Android 10 → 9" (o que foi feito)
O projeto usava **.NET 10** (`net10.0-android`). Isso foi trocado para **.NET 9** (`net9.0-android`),
que funciona no Visual Studio 2022 mais recente.

- `NeuroRotina.csproj`: todos os `net10.0-*` viraram `net9.0-*`; `Microsoft.Extensions.Logging.Debug` → 9.0.0;
  removido o `MauiXamlInflator=SourceGen` (recurso do .NET 10).
- Código ajustado para compilar no .NET 9: no .NET 10 o `DatePicker.Date`/`TimePicker.Time` aceitam nulo,
  no .NET 9 não. As telas `CriancaPage` e `RotinaPage` agora usam uma forma que funciona nos dois.
- O app **roda em celular com Android 9**: a versão mínima do Android continua 5.0 (API 21).
  Se você usa emulador, crie um com imagem **Android 9 (API 28)**.
- Se o Visual Studio não abrir o `NeuroRotina.slnx`, abra o `NeuroRotina.sln` (mesmo projeto).
- Requisito: Visual Studio 2022 17.12 ou mais novo, com a carga de trabalho ".NET Multi-platform App UI".

## 2. Design do Figma
- `Resources/Styles/Tema.xaml`: cores (#317597 azul, #D9D9D9 fundo, Água #006ED4, Banheiro #FF0000,
  Brincar #FFCC00, Comer #009C6E), títulos serifados, botões, campos e cartões.
- `Views/CabecalhoView`: faixa azul do topo com título e botão de voltar (usada em todas as telas).
- App em tema claro, como no Figma. Barra de status, ícone e splash agora são azuis.

## 3. Espaço do logo
O login (e cadastro / login da criança) tem um quadro "SEU LOGO AQUI".
Para trocar: copie sua imagem para `NeuroRotina/Resources/Images/logo.png` (nome em minúsculas).
Ela aparece sozinha por cima do quadro. Nada mais precisa mudar.

## 4. Rotinas com alerta semanal (login do cuidador → login da criança)
- **Nova Rotina**: título, horário de início, dias da semana (D S T Q Q S S), alerta ligado/desligado,
  som (Alarme, Aviso suave, Só vibrar) e descrição.
- Os alertas usam o AlarmManager do Android, como um despertador: tocam com o app fechado,
  repetem toda semana e são reagendados quando o celular reinicia (`Platforms/Android/*`).
- A criança vê **só as rotinas do dia**, em ordem de horário, com botão 🔊 (lê a rotina em voz alta),
  passo a passo marcável e botão "MARCAR COMO FEITO". A conclusão é **por dia**, então a rotina
  volta desmarcada no dia seguinte.
- O cuidador pode editar e excluir rotinas e atividades (excluir também cancela o alerta).

## 5. Necessidades (Comunicação)
Tela com os 4 cartões do Figma (Água, Banheiro, Brincar, Comer) + botão Ajuda.
Ao tocar: o celular **fala a frase** ("Eu quero beber água."), mostra a confirmação e salva o pedido.
Os ícones são SVG em `Resources/Images/ic_*.svg` (pode trocar por pictogramas, mantendo o nome).

## 6. Histórico Diário (cuidador)
Na lista de crianças, botão "Histórico diário". Mostra um resumo automático de hoje
(rotinas feitas e pedidos de comunicação) e os registros no formato do Figma
(Data / Comportamento / Observação). "Novo registro" já vem com o resumo do uso da prancha.

## Como testar o alarme
1. Entre como adulto, cadastre uma criança, abra "Rotinas e alertas".
2. Crie uma rotina para **daqui a 2 minutos**, marque o dia de hoje e deixe o alerta ligado.
3. Salve (no Android 13+ aceite a permissão de notificações).
4. Feche o app e aguarde: deve tocar o alarme.
Em alguns celulares (Xiaomi, Samsung, Motorola) é preciso tirar o NeuroRotina da
"otimização de bateria" para o alarme tocar com a tela apagada.

## Limitações conhecidas (próximos passos)
- Os dados ficam **no aparelho** (SQLite). Os dois logins funcionam no **mesmo celular**.
  Para cuidador e criança em celulares diferentes é preciso um servidor (ex.: Firebase / API).
- O alarme é uma notificação com som de alarme. Uma tela de alarme em tela cheia que toca até a
  criança desligar ainda não existe.
- As senhas do adulto são salvas em texto simples no banco. Recomendo guardar um hash antes de publicar.
- O projeto não foi compilado no ambiente em que foi escrito (sem .NET SDK). Se aparecer
  qualquer erro de build, envie a mensagem para ajuste.
