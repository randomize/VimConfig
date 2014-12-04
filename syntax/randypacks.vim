" Vim syntax file
" Language:	randy package list file
" Maintainer:	Mihailenco Eugene <mihailencoe@gmail.com>
" Last Change:	2014 Sep 6

" Quit when a (custom) syntax file was already loaded
if exists("b:current_syntax")
  finish
endif

syn keyword	randypackTodo	contained TODO FIXME XXX

syn match       randypackComment      "\/\/.*" contains=randypackTodo,@Spell
syn match       randypackHeader	      ".*\:$"
" syn match       randypackLocalPack    "^.*\-git "
" syn match       randypackLocalPack    "^.*\-svn "
syn match       randypackLocalPack    "^\s*aur\/\S* "
syn match       randypackLocalPack    "^\s*local\/\S* "

hi def link randypackComment     Comment
hi def link randypackTodo        Todo
hi def link randypackHeader      Underlined
hi def link randypackLocalPack   Special

let b:current_syntax = "randypack"

" vim: ts=8 sw=2
