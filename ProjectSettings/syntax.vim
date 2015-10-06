" Colorscheme created with ColorSchemeEditor v1.2.3
"Name: wip
"Maintainer: Demelev
"Last Change: 2015 Sep 8
"
"set background=dark
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

hi CTagsInterface guifg=#00ff00 gui=NONE
hi CTagsGlobalVariable guifg=#c0c0c0 gui=NONE
hi CTagsLocalVariable guifg=#c0c0c0 gui=NONE
hi CTagsMember guifg=#c0c0c0 gui=NONE
hi CTagsEvent guifg=#c0c0c0 gui=NONE
hi CTagsMethod guifg=#ff8000 gui=NONE
hi CTagsVariable guifg=#c0c0c0 gui=NONE
hi CTagsFunction guifg=#ff8000 gui=NONE
hi CTagsClass guifg=#ffd700 gui=NONE
hi CTagsStructure guifg=#ffd700 gui=NONE
hi CSharpUserTypes guifg=#ffd700 gui=NONE
hi EnumerationValue guifg=#7F7F7F gui=NONE
hi csNewType guifg=#ffd700 gui=NONE
hi csRepeat  guifg=#1E90FF gui=NONE
hi csContextualStatement guifg=#1E90FF gui=NONE
hi csType guifg=#1E90FF gui=NONE
hi csModifier guifg=#1E90FF gui=NONE
hi StorageClass guifg=#1E90FF gui=NONE
