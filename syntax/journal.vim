" Vim syntax file
" Language: Jrnl syntax
" Maintainer:	Mihailenco Eugene <mihailencoe@gmail.com>
" Last Change:	2016-02-20
" Version: 1.0.0

if version < 600
  syntax clear
elseif exists("b:current_syntax")
  finish
endif

setlocal iskeyword+=:
syn case ignore

syn match date "^[0-9]\{4}-[0-9]\{2}-[0-9]\{2}"
syn match time "^[0-9]\{4}-[0-9]\{2}-[0-9]\{2} [0-9]\{2}:[0-9]\{2}"
syn match at " @[_-a-zA-Z0-9]\+"
syn match hash " #[_-a-zA-Z0-9]\+"
syn match regular "\[ ]"
syn match code " `\S\+`"
syn match code2 "^\$\s.\+$"
syn match completed "\[x] .\+"

syn match url /https\?:\/\/\(\w\+\(:\w\+\)\?@\)\?\([A-Za-z][-_0-9A-Za-z]*\.\)\{1,}\(\w\{2,}\.\?\)\{1,}\(:[0-9]\{1,5}\)\?\S*/

highlight def link hash Number
highlight def link url Number
highlight def link at Keyword
highlight def link date Function
highlight def link time Function
highlight def link regular Comment
highlight def link completed Comment
highlight def link code String
highlight def link code2 String

let b:current_syntax = "journal"
