# FastFoodPayment


O repositorio FastFoodPayment tem por objetivo implementar uma Lambda Function respons�vel por lidar apenas com o meio de pagamento por QRCode pelo MercadoPago.

## Vari�veis de ambiente
Todas as vari�veis de ambiente do projeto visam fazer integra��o com algum servi�o da AWS ou MercadoPago. Explicaremos a finalidade de cada uma:

- AWS_ACCESS_KEY_DYNAMO: "Access key" da AWS. Recurso gerado no IAM para podermos nos conectar aos servi�os da AWS;
- AWS_SECRET_KEY_DYNAMO: "Secret key" da AWS. Recurso gerado no IAM para podermos nos conectar aos servi�os da AWS. Deve ser utilizado corretamente com seu par AWS_ACCESS_KEY_DYNAMO;
- AWS_TABLE_NAME_DYNAMO: Tablea do dynamo utilizada por este servi�o para salvar os dados do pagamento.
- AWS_SQS_LOG: Url da fila de log no SQS da AWS.
- AWS_SQS_GROUP_ID_LOG: Group Id da fila de log no SQS da AWS.
- AWS_SQS_PRODUCTION: Url da fila de enviar pedidos para produ��o no SQS da AWS.
- AWS_SQS_GROUP_ID_PRODUCTION: Group Id da fila de enviar pedidos para produ��o no SQS da AWS.
- BASE_URL_MERCADO_PAGO: Url da api do emrcado pago
- ACCESS_TOKEN_MERCADO_PAGO: Access Token gerado pelo merdao pago para poder acessar a API deles.
- USER_ID_MERCADO_PAGO: Id de usu�rio no mercado pago.
- EXTERNAL_POS_ID_MERCADO_PAGO: Id externo da loja aberta no mercado pago.



## Execu��o do projeto

A execu��o do projeto pode ser feita buildando o dockerfile na raiz do reposit�rio e depois executando a imagem gerada em um container. O servi�o foi testado sendo executado direto pelo visual Studio e pela AWS.

## Testes

Conforme foi solicitado, estou postando aqui as evid�ncias de cobertura dos testes. A cobertura foi calculada via integra��o com o [SonarCloud](https://sonarcloud.io/) e pode ser vista nesse [link](https://sonarcloud.io/organizations/techchallengefernandomelim/projects). A integra��o com todos os reposit�rios poder� ser vista nesse link.

![Coverage1](./images/coverage1.png)

![Coverage2](./images/coverage2.png)

Atrav�s das imagens � poss�vel observar que a cobertura por testes unit�rios ficou superior a 80%, conforme solicitado.

## Endpoints

Os endpoints presentes nesse projeto s�o:

- POST /CreatePayment: Respons�vel por criar o pedido no Mercado Pago e retornar o QR Code(PIX) para pagamento do pedido. Esse endpoint � chamado quando o cliente est� criando um pedido. 
- PATCH /UpdatePayment/{in_store_order_id}: Respons�vel por atualizar o status do pagamento do pedido e por enfileirar o pedido para a produ��o caso o mesmo tenha sido pago.