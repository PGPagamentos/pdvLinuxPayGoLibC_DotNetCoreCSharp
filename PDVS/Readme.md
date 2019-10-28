
Instruções atualizadas para executar o aplicativo no Ubuntu:

1) O link abaixo tem as instruções para instalar o mono-develop (compilador :decepcionado:
https://www.mono-project.com/download/stable/
b) Para instalar a IDE :
sudo apt-get install monodevelop
c) Para configurar o ambiente de execução
   Entrar num  terminal.
   digitar: su e informar senha do root.
   PINPAD :
     - conectar o Pinpad e Pesquisar em qual USB ele esta conectado
     - utilizando comando: dmesg  serão listadas muitas informações, procurar por GERTEC, ao encontrar criar permissões totais na pasta /dev para a conexão USB. exemplo de um PinPad na USB:
     - ttyACM0: cd /dev = pasta com todas as portas chmod 777 ttyACM0 = permissões totais ls -l = lista todas as permissõs da pasta /dev
   Shared Object :
          - digitar: su e informar senha do root.
          - copiar a so  PGWebLib.so  para /usr/lib
          - copiar certificado certificado.crt para o diretoŕio de execução da aplicação
     d) Para fazer a instalação clicar no botão instala da aplicação.
     e)
