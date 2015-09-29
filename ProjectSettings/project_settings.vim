"
"Tags settings
"  -->
"let g:TagHighlightSettings['UserLibraryDir'] = expand("~/.vim/highlight")
"getcwd()

let g:unityTags   = ["unity5", ["~/.vim/tags/unity5/"]]

let g:dotNetTags   = ["dotNet", ["~/.vim/tags/dotNet/"]]

let g:thirdTags   = [unityTags, dotNetTags]

let g:excludeDirs = ['Library/',
            \'ProjectSettings/']

let g:projectTagsList = ['Assets']

"let g:ctagsOptions = '--tag-relative=yes -R -f .vim/tags --c++-kinds=+p --fields=+iaS --extra=+q '
"let g:ctagsOptions = '--tag-relative=yes -R -f .vim/tags --c#-kinds=+p --fields=+iaS --extra=+q '
let g:ctagsOptions = '-R --c\#-kinds=+cimnp --fields=+ianmzS --extra=+fq '

"  <--
let prog = 'set makeprg='.g:projectDir.'\.vim\buildProject.bat'
execute prog

set errorformat=\ %#%f(%l):\ %m
set errorformat+=\ %#%f(%l\,%c):\ %m

"call ExtractSnips(".vim\snippets", "cpp")
set wildignore+=*\\.git\\*,*\\.hg\\*,*\\.svn\\*
