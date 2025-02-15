# language: pt

Funcionalidade: Processar Mensagem 

  Cenário: Processamento de Mensagem Bem-Sucedido apaga mensagem da fila
    Dado o pedido é atualizado com sucesso  
    Quando uma mensagem válida é processada
    Então a mensagem deve ser excluída da fila

  Cenário: Processamento de Mensagem Bem-Sucedido não posta mensagem na DLQ
    Dado o pedido é atualizado com sucesso  
    Quando uma mensagem válida é processada
    Então a mensagem não é enviada para a DLQ

  Cenário: Falha na atualização do pagamento posta mensagem na DLQ
    Dado pedido falha ao ter o pagamento atualizado  
    Quando uma mensagem válida é processada
    Então a mensagem é enviada para a DLQ
  
  Cenário: Falha na deserialização da mensagem posta mensagem na DLQ
    Quando uma mensagem inválida é processada
    Então a mensagem é enviada para a DLQ