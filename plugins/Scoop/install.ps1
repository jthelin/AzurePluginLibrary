(get-psprovider 'FileSystem').Home = 'c:\applications\'
iex (new-object net.webclient).downloadstring('https://get.scoop.sh')