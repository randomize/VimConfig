"if exists('+colorcolumn')
    "highlight ColorColumn ctermbg=235 guibg=#252525
    ""highlight CursorLine ctermbg=235 guibg=#2a2b25
    ""highlight CursorColumn ctermbg=235 guibg=#2a2b25
    "let &colorcolumn=+1
    ""join(range(81,400),",")
    ""set cc=+1
"else
    "autocmd BufWinEnter * let w:m2=matchadd('ErrorMsg', '\%>80v.\+', -1)
"end

function! WrapRegion(first, last)
    call append(a:last, "#endregion")
    call append(a:first-1, "#region ")
    call cursor(a:first, "$")
    :startinsert!
endfunction

function! WrapRegionNamed(first, last, name)
    call append(a:last, "#endif")
    call append(a:first-1, "#if ".a:name)
endfunction

command! -range -nargs=* WrapWithRegion call WrapRegion(<line1>, <line2>)
command! -range -nargs=1 WrapWithIf call WrapRegionNamed(<line1>, <line2>, <args>)
