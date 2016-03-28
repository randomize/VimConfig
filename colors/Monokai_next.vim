" Colorscheme created with ColorSchemeEditor v1.2.3
"Name: Monokai_next
"Maintainer: 
"Last Change: 2015 Oct 15
set background=dark
if version > 580
	highlight clear
	if exists("syntax_on")
		syntax reset
	endif
endif
let g:colors_name = "Monokai_next"

if v:version >= 700
	highlight CursorColumn guibg=#3c3d37 gui=NONE
	highlight CursorLine guibg=#3c3d37 gui=NONE
	highlight Pmenu guifg=#828282 guibg=#191919 gui=underline
	highlight PmenuSel guifg=#aaff00 guibg=#3e3e3e gui=underline
	highlight PmenuSbar guibg=Grey gui=NONE
	highlight PmenuThumb guibg=White gui=NONE
	highlight TabLine guibg=DarkGrey gui=underline
	highlight TabLineFill gui=reverse
	highlight TabLineSel gui=bold
	if has('spell')
		highlight SpellBad gui=undercurl
		highlight SpellCap gui=undercurl
		highlight SpellLocal gui=undercurl
		highlight SpellRare gui=undercurl
	endif
endif
highlight Cursor guifg=#272822 guibg=#f8f8f0 gui=NONE
highlight CursorIM gui=NONE
highlight DiffAdd guifg=#f8f8f2 guibg=#46830c gui=bold
highlight DiffChange guifg=#f8f8f2 guibg=#243955 gui=NONE
highlight DiffDelete guifg=#8b0807 guibg=DarkCyan gui=NONE
highlight DiffText guifg=#f8f8f2 guibg=#204a87 gui=bold
highlight Directory guifg=#ae81ff gui=NONE
highlight ErrorMsg guifg=#f8f8f0 guibg=#f92672 gui=NONE
highlight FoldColumn guifg=Cyan guibg=Grey gui=NONE
highlight Folded guifg=#75715e guibg=#272822 gui=NONE
highlight IncSearch guifg=#272822 guibg=#e6db74 gui=NONE
highlight LineNr guifg=#90908a guibg=#3c3d37 gui=NONE
highlight CursorLineNr guifg=#9BFF00 guibg=#292929 gui=NONE
highlight MatchParen guifg=#f92672 guibg=DarkCyan gui=underline
highlight ModeMsg gui=bold
highlight MoreMsg guifg=SeaGreen gui=bold
highlight NonText guifg=#49483e guibg=#31322c gui=NONE
highlight Normal guifg=#f8f8f2 guibg=#272822 gui=NONE
highlight Question guifg=Green gui=bold
highlight Search guifg=#D0FF36 guibg=NONE gui=reverse,italic
highlight link SignColumn LineNr
highlight SpecialKey guifg=#49483e guibg=#3c3d37 gui=NONE
highlight StatusLine guifg=#f8f8f2 guibg=#64645e gui=bold
highlight StatusLineNC guifg=#f8f8f2 guibg=#64645e gui=NONE
highlight Title guifg=#f8f8f2 gui=bold
highlight VertSplit guifg=#64645e guibg=#64645e gui=NONE
highlight Visual guibg=#49483e gui=NONE
highlight VisualNOS gui=bold,underline
highlight WarningMsg guifg=#f8f8f0 guibg=#f92672 gui=NONE
highlight WildMenu guifg=Black guibg=Yellow gui=NONE
highlight Boolean guifg=#ae81ff gui=NONE
highlight Character guifg=#ae81ff gui=NONE
highlight Comment guifg=#75715e gui=NONE
highlight Conditional guifg=#f92672 gui=NONE
highlight Constant guifg=#ffa0a0 gui=NONE
highlight link Debug Special
highlight Define guifg=#66D9EF ctermfg=81 gui=NONE
highlight link Delimiter Special
highlight Error guifg=White guibg=Red gui=NONE
highlight link Exception Statement
highlight Float guifg=#ae81ff gui=NONE
highlight Function guifg=#a6e22e gui=NONE
highlight Identifier guifg=#66d9ef gui=italic
highlight Ignore guifg=bg gui=NONE
highlight link Include PreProc
highlight Keyword guifg=#f92672 gui=NONE
highlight Label guifg=#e6db74 gui=NONE
highlight link Macro PreProc
highlight Number guifg=#ae81ff gui=NONE
highlight Operator guifg=#f92672 gui=NONE
highlight link PreCondit PreProc
"highlight PreProc guifg=#f92672 gui=NONE
highlight PreProc gui=bold guifg=#A6E22E
highlight link Repeat Statement
highlight Special guifg=#f8f8f2 gui=NONE
highlight link SpecialChar Special
highlight link SpecialComment Special
highlight Statement guifg=#f92672 gui=NONE
highlight StorageClass guifg=#2E90FF gui=italic
highlight Class guifg=#F5FF2D
highlight Method guifg=#FFA500 gui=NONE
highlight String guifg=#e6db74 gui=NONE
highlight Property guifg=#e6db74 gui=NONE
highlight Member guifg=#e6db74 gui=NONE
highlight Field guifg=#ffffff gui=NONE
highlight link Structure Type
highlight Tag guifg=#f92672 gui=NONE
highlight Todo guifg=Yellow guibg=NONE gui=NONE
highlight SyntasticWarningSign guifg=Yellow guibg=#3c3d37 gui=NONE
highlight SyntasticErrorLine guibg=#440000 gui=NONE
highlight SyntasticErrorSign guifg=#ff0000 guibg=#3c3d37 gui=NONE

highlight Type guifg=#2E90FF gui=NONE
highlight link Typedef Type
highlight Underlined guifg=#80a0ff gui=underline
highlight csAttributeName guifg=#008040 gui=NONE


"ColorScheme metadata{{{
if v:version >= 700
	let g:Monokai_next_Metadata = {
				\"Palette" : "#000000:#9BFF00:#7F7F7F:#FF0000:#A020F0:#0000FF:#ADD8E6:#00FF00:#FFFF35:#FFA500:#E6E6FA:#A52A2A:#8B6914:#1E90FF:#FFC0CB:#90EE90:#1A1A1A:#4D4D4D:#BFBFBF:#E5E5E5",
				\"Last Change" : "2015 Oct 15",
				\"Name" : "Monokai_next",
				\}
endif
"}}}
" vim:set foldmethod=marker expandtab filetype=vim:
