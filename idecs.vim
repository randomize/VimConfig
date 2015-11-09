" Конфигурационный файл Vim IDE

" Запрещаем восстановление настроек из сессии,
" т. к. тогда при изменении ~/.vimrc даже после
" перезагрузки IDE новые настройки не будут
" вступать в силу.
set sessionoptions-=options

" Добавляем пути к библиотекам
"set path+=/usr/include/gtk-2.0
"set path+=./netlog

" При закрытии Vim'а сохраняем информацию о текущей сессии
"au VimLeave * :mksession! .vim/ide.session

let g:projectDir = getcwd()

" Загружаем ранее сохраненную сессию -->
"if getfsize(".vim/ide.session") >= 0
    "silent source .vim/ide.session
"endif

" Загружаем ранее сохраненную сессию <--
exec "cd ".g:projectDir

"""
" Добавляем путь в runtimepath
let addRuntime = "set runtimepath+=".g:projectDir."/.vim"
exec addRuntime

let g:projectTagsFile = "tags"
"""
"" Загружаем настройки данного проекта
if getfsize(".vim/project_settings.vim") >= 0
    source .vim/project_settings.vim
endif

exec 'set tags+='.g:projectTagsFile


""Устанавливает правила синтаксиса, специфичные для данного проекта.
"" -->
    function! SetProjectSyntax()
        if getfsize(".vim/syntax.vim") >= 0

            source .vim/syntax.vim
        endif
    endfunction
" <--
call SetProjectSyntax()

if getfsize(".vim\project_snippets.snippets") >= 0
    ExtractSnipFile(".vim", "project_snippets.snippets");
endif
"Устанавливаем цветовую схему для этого проекта
" -->
    function! SetProjectColors()
        if getfsize(".vim/colorsceme.vim") >= 0
            source .vim/colorsceme.vim
        endif
    endfunction
" <--
call SetProjectColors()

" working with ctasg around tags
"
function! UpdateTags()
  let fullpath = expand("%:p")
  exec 'cd '.g:projectDir
  let cwd = g:projectDir.'/'
  let filePath = substitute(fullpath, escape(cwd, '.\'), "", "")
  let escapedFilePath = escape(filePath, '/.')
  let escapedFilePath = substitute(escapedFilePath, '\\/', '\\\\/', "g")
  let command = "Dispatch ~/.vim/updatetags ".escapedFilePath.' "'.filePath.'" '.v:servername
  "echo command
  silent execute command
endfunction

command! UpdateProjectHighlight call UpdateTags()

function! UpdateMainTags()
    let cwd = g:projectDir.'/'
    exec "cd ".cwd
    let command = "silent !ctags -f ".cwd.g:projectTagsFile." ".g:ctagsOptions

    for i in g:excludeDirs
        let command .= ' --exclude="'.i.'"'
    endfor

    for i in g:projectTagsList
        let command .= ' "'.i.'"'
    endfor
    exec command
    UpdateTypesFileOnly
endfunction
command! UpdateMainTags call UpdateMainTags()

function! UpdateThirdTags( name )
    for third in g:thirdTags
        let filepath = expand("~/.vim/tags/".a:name.".tags")
        if third[0] == a:name
            let command = ' silent !ctags '.g:ctagsOptions.' -f ' . filepath 
            for i in third[1]
                let command .= ' "'.expand(i).'"'
            endfor
            exec command
        endif
    endfor
endfunction

autocmd BufWritePost *.cs,*.cpp,*.h,*.c,*.hpp call UpdateTags()
"autocmd BufReadPost * :call SetProjectSyntax()| call SetProjectColors()

" connect tags lists
let g:TagHighlightSettings['UserLibraries'] = []
let g:TagHighlightSettings['UserLibraryDir'] = expand("~/").".vim/highlight"
let s:libraries = g:TagHighlightSettings['UserLibraries']

function! HLFileName(lib_name)
    return expand(g:TagHighlightSettings['UserLibraryDir']."/".a:lib_name.".taghl")
endfunction

function! UpdateLibraryHL(lib_name)
    let g:TagHighlightSettings['TagFileName'] = expand('~/.vim/tags/'.a:lib_name.".tags")
    let g:TagHighlightSettings['TypesFileNameForce'] = HLFileName(a:lib_name)
    UpdateTypesFileOnly
endfunction

function! CheckHLFile(lib_name)
    let hl_filename = HLFileName(a:lib_name)
    if getfsize(hl_filename) == -1
        call UpdateLibraryHL(a:lib_name)
    endif
endfunction

for thirdLibrary in g:thirdTags 
    call add(s:libraries, thirdLibrary[0].'.taghl')
    call CheckHLFile(thirdLibrary[0])

    let filename = expand("~/.vim/tags/".thirdLibrary[0].".tags")
    if getfsize(filename) >= 0
        exec 'set tags+='.filename
    endif
endfor


function! UpdateLibrariesHL()
    for tl in g:thirdTags
        let g:TagHighlightSettings['TagFileName'] = expand('~/.vim/tags/'.tl[0].".tags")
        let g:TagHighlightSettings['TypesFileNameForce'] = expand("~/.vim/highlight/".tl[0].".taghl")
        UpdateTypesFileOnly
    endfor

    let g:TagHighlightSettings['TagFileName'] = g:projectTagsFile
    let g:TagHighlightSettings['TypesFileNameForce'] = "types_cs.taghl"
endfunction

" -->
function! UpdateLibrariesTags()
    for tl in g:thirdTags
        call UpdateThirdTags(tl[0])
    endfor

    call UpdateLibrariesHL()
endfunction
command! UpdateLibrariesTags call UpdateLibrariesTags()

let g:TagHighlightSettings['TagFileName'] = g:projectTagsFile
let g:TagHighlightSettings['TypesFileNameForce'] = "types_cs.taghl"

set dict+=~/.vim/dicts/unity3d.dict
"-->
"function! UpdateProjectHL()

"endfunction
ReadTypes
