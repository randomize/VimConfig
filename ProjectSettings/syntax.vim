" Colorscheme created with ColorSchemeEditor v1.2.3
"Name: wip
"Maintainer: Demelev
"Last Change: 2015 Sep 8
"
set background=dark
"if version > 580
"	highlight clear
"	if exists("syntax_on")
"		syntax reset
"	endif
"endif
"let g:colors_name = "wip"

if v:version >= 700
	highlight CursorColumn guibg=#262626 gui=NONE
	highlight CursorLine gui=NONE guibg=#222222
	highlight Pmenu guifg=#828282 guibg=#191919 gui=underline
	highlight PmenuSel guifg=#aaff00 guibg=#3e3e3e gui=underline
	highlight PmenuSbar guibg=Grey gui=NONE
	highlight PmenuThumb guibg=White gui=NONE
	highlight TabLine guifg=#a09998 guibg=#202020 gui=underline
	highlight TabLineFill guifg=#a09998 guibg=#202020 gui=underline
	highlight TabLineSel guifg=#a09998 guibg=#404850 gui=underline
	if has('spell')
		highlight SpellBad gui=undercurl
		highlight SpellCap gui=undercurl
		highlight SpellLocal gui=undercurl
		highlight SpellRare gui=undercurl
	endif
endif

highlight CTagsGlobalVariable guifg=#c0c0c0 gui=NONE
highlight CTagsLocalVariable guifg=#c0c0c0 gui=NONE
highlight CTagsMember guifg=#c0c0c0 gui=NONE
highlight CTagsMethod guifg=#ff8000 gui=NONE
highlight CTagsVariable guifg=#c0c0c0 gui=NONE
highlight CTagsFunction guifg=#ff8000 gui=NONE
highlight CTagsClass guifg=#ffd700 gui=NONE
highlight CSharpUserTypes guifg=#ffd700 gui=NONE
highlight EnumerationValue guifg=#7F7F7F gui=NONE
highlight csNewType guifg=#ffd700 gui=NONE
highlight csType guifg=#1E90FF gui=NONE
highlight csModifier guifg=#1E90FF gui=NONE
hi StorageClass guifg=#1E90FF gui=NONE
