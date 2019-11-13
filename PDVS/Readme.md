
Instruções atualizadas para executar o aplicativo no Ubuntu:

1) O link abaixo tem as instruções para instalar o mono-develop (compilador )
   https://www.mono-project.com/download/stable/#download-lin
       No Item :
          Ubuntu 16.04 (i386, amd64, armhf, arm64, ppc64el)


2) Para instalar a IDE, você pode executar diretamente por linha de comando:

       - sudo apt-get install monodevelop

ou, entrar via site:

    - A versão da ide deve ser a 5.10 que está também no site: 
    https://ubuntu.pkgs.org/16.04/ubuntu-universe-amd64/monodevelop_5.10.0.871-2_all.deb.html

3) Para testar se o compilador foi instalado, executar o comando abaixo como usuário normal:

       - monodevelop

4) Para configurar o ambiente de execução da aplicação, entrar num terminal e digitar: 

       - sudo su
e informar senha do root
   
Com o PINPAD:
  
conectar o Pinpad e pesquisar em qual USB ele esta conectado. Digitar o comando:
 
     - dmesg
 
procurar na lista por **GERTEC**, ao encontrar criar permissões totais na pasta /dev para a conexão USB. Exemplo de um PinPad na USB:
* cdc_acm 1-1:2.0: ttyACM0: USB ACM device *

entrar no diretório /dev

    - cd /dev

para ver se o arquivo/pasta representa o hardware:

    - ls -l ttyACM0

para atribuir permissões totais:

    - chmod 777 ttyACM0

para verificar se a atribuição ficou correta, liste novamente (ls -l ttyACM0):

* root@deliver-lenovo-ideapad-310-14isk-zago:/dev# ls -l ttyACM0 *
* crwxrwxrwx 1 root dialout 166, 0 nov 13 15:02 ttyACM0 *


5) Shared Object:

digitar: 

    - sudo su
   
e informar senha do root

fazer Download do lib, e copiar a lib **PGWebLib.so**  para /usr/lib

    - root@user:/home/user/Downloads# cp PGWebLib.so /usr/lib/.

verificar se o certificado **certificado.crt** está no diretoŕio de execução da aplicação

    - root@user:/home/user/Downloads/pdvLinuxPayGoLibC_DotNetCoreCSharp-master# ls -l PDVS/bin/Debug/certificado.crt 
    - -rw-rw-r-- 1 eduardo_zago eduardo_zago 1537 nov 13 15:01 PDVS/bin/Debug/certificado.crt

antes de instalar, **deletar o diretório PGWebLib** que fica no diretorio de execução da aplicação. Se estiveres debugando, o diretório está no path "PDVS/bin/Debug/".

Para fazer a instalação do **PDC**, clicar no botão instala da aplicação.


6) Para Testar a Venda Selecionar PWOPER_SALE no combo box PWINFO_OPERATION e ativar botão Executa.

