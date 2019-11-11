
Instruções atualizadas para executar o aplicativo no Ubuntu:

1) O link abaixo tem as instruções para instalar o mono-develop (compilador )
https://www.mono-project.com/download/stable/

- Para instalar a IDE :
sudo apt-get install monodevelop
- A versão da ide deve ser a 5.10 que está também no site : https://ubuntu.pkgs.org/16.04/ubuntu-universe-amd64/monodevelop_5.10.0.871-2_all.deb.html

- Para configurar o ambiente de execução
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
- Para fazer a instalação clicar no botão instala da aplicação.
- OBS :  antes de instalar deletar diretório PGWebLib que fica no diretorio de execucao da aplicacao
         se estiver debugando está em PDVS/bin/Debug/ 

2) Para Testar a Venda Selecionar PWOPER_SALE no combo box PWINFO_OPERATION e ativar botão Executa
