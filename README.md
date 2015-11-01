# Unity Vim Config at Bully! Entertainment

![Image of startify]
(https://github.com/randomize/VimConfig/blob/master/docs/title.png)

Vim configuration files. These are shared between developers in our company.
Env variable `$bully_dev` is used for setting up personal config preferences.
Config is mostly oriented to Unity development, but also has C++/Rust simple setup.
Tested to run on GNU/Linux, Mac OS and Windows.


[Vundle] (http://github.com/gmarik/Vundle.vim) is used as package manager



## Plugins used


* [YouCompleteMe](http://github.com/Valloric/YouCompleteMe)
* [OmniSharp](http://github.com/OmniSharp/omnisharp-vim)
* [UltiSnips](http://github.com/SirVer/ultisnips)
* [ControlP](http://github.com/ctrlpvim/ctrlp.vim) && [Unite](http://github.com/Shougo/unite.vim)
* [Syntastic](http://github.com/scrooloose/syntastic)
* and many many others

## Features

* Extended semantic coloring for C# and Unity specific classes
* Unity C\# snippets

## Setup procedure

(TODO: fill-in detailed steps):

1. git clone https://github.com/VundleVim/Vundle.vim.git ~/.vim/bundle/Vundle.vim
2. Compile vimproc
3. Compile ycm
4. Compile omnisharp
5. Setup `$bully_dev` to indicate username

## Hotkeys
(TODO: fill-in)

### Main
Action | Hotkey
------------ | -------------
Remove trailing whitespaces | `<leader>rtw`
Ack | `<leader>a`

### Unity and C# #
Action | Hotkey
------------ | -------------
Goto definition | `<leader>sg`
Format the code | `<leader>sf`


## WIP
* Unity debugger integration
* Migrate extended syntax to omnisharp powered
* Migrate to Roslyn

## License
GNU GPL Version 3
