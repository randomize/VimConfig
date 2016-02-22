" Vim config
"   vim: foldmethod=marker
"
"   TODO:
"
"   1) play with args and argdo commands (like :args `ag -l foo`)
"   2) quickfix commands (go, t, i etc)
"   3) Unite setup : file_mru outline history/yank - why not working?
"   4) unimpaired keys learn
"   5) vim-multiple-cursors
"   6) vim-easy-align
"   7) vim-operator-user
"   8) vim-perforce
"
"   Authors:
"   Mihailenco Eugene, Dmitrii Emeleov, Dmitrii Stavila
"
"   Credits:
"   Derek Wyatt, Tim Pope, Vimawesome.com
"
" =========================================================================
" OS Detector and global swithches
" =========================================================================
" {{{
"


if !exists("g:os")
    if has("win64") || has("win32") || has("win16")
        let g:os = "Windows"
    elseif  has('win32unix')
        let g:os = "Cygwin"
    else
        let g:os = substitute(system('uname'), '\n', '', '')
        " Darwin and Linux most common values for OSX and GNU/Linux
    endif
endif

if !exists("g:bully_dev")
    let g:bully_dev = $bully_dev
    "let g:bully_dev = "dstavila"
    " let g:bully_dev = "demelev"
endif

" }}}
"
" =========================================================================
" Vundle
" =========================================================================
" {{{
"
set nocompatible
filetype off
set rtp+=~/.vim/bundle/Vundle.vim

if g:os == "Windows"
    set rtp+=~/.vim
endif

call vundle#begin()

Plugin 'Cofyc/vim-uncrustify'
Plugin 'vim-scrips/vim-coffee-script'
Plugin 'ryanoasis/vim-devicons'
Plugin 'Buffergator'
Plugin 'vim-scripts/dbext.vim'
Plugin 'ludovicchabant/vim-ctrlp-autoignore'

" let Vundle manage Vundle
Plugin 'gmarik/Vundle.vim'

Plugin 'WebAPI.vim'
Plugin 'open-browser.vim'
" ColorSchemeEditor
Plugin 'ColorSchemeEditor'

" CtrlP
Plugin 'ctrlpvim/ctrlp.vim'

" TagHighlight
Plugin 'demelev/TagHighlight'

" cpp/h switch
Plugin 'derekwyatt/vim-fswitch'

" creates C++ class method implementations
Plugin 'derekwyatt/vim-protodef'

" Todo plugin
Plugin 'neochrome/todo.vim'
" Plugin 'vitalk/vim-simple-todo'
Plugin 'vim-scripts/TaskList.vim'

" Git support
Plugin 'tpope/vim-fugitive'
Plugin 'airblade/vim-gitgutter'

" Mercurial support
Plugin 'mhinz/vim-signify'

Plugin 'tpope/vim-dispatch'

" Super increment
Plugin 'VisIncr'

" Auto completion
Plugin 'Valloric/YouCompleteMe'

" Snippets
Plugin 'SirVer/ultisnips'
Plugin 'honza/vim-snippets'

" Plugin to show syntax errors
Plugin 'scrooloose/syntastic'

" LaTeX
Plugin 'LaTeX-Box-Team/LaTeX-Box'

" Easy motion
Plugin 'Lokaltog/vim-easymotion'

" Commenting
if g:bully_dev == "demelev" || g:bully_dev == "dstavila"
    Plugin 'scrooloose/nerdcommenter'
elseif g:bully_dev == "eugene"
    Plugin 'tomtom/tcomment_vim'
endif

" Unite - obsoletes: command-t, fuzzy-finder and buffer explorer
Plugin 'Shougo/vimproc.vim'
Plugin 'Shougo/unite.vim'
Plugin 'ujihisa/unite-colorscheme'
Plugin 'Shougo/neoyank.vim'
Plugin 'Shougo/neomru.vim'
Plugin 'unite-yarm'

" Indent guides
Plugin 'nathanaelkane/vim-indent-guides'

" Tabling
Plugin 'dhruvasagar/vim-table-mode'
Plugin 'godlygeek/tabular'

" Status line
Plugin 'bling/vim-airline'

" Session save/restore
Plugin 'xolox/vim-session'
Plugin 'xolox/vim-misc'

" Undo visual tree
" Plugin 'sjl/gundo.vim' " Older one
Plugin 'mbbill/undotree'

" Tags for C++/C and others
Plugin 'demelev/tagbar'

" Formatting with clanfg format
Plugin 'rhysd/vim-clang-format'

" Taglist
Plugin 'vim-scripts/taglist.vim'

" Rainbow prentheses
Plugin 'kien/rainbow_parentheses.vim'

" ==== SYNTAX =========================
Plugin 'vim-scripts/ck.vim'
Plugin 'vim-scripts/glsl.vim'
Plugin 'vim-scripts/cg.vim'
Plugin 'leshill/vim-json'
Plugin 'chrisbra/csv.vim'
Plugin 'dag/vim2hs'
Plugin 'tpope/vim-markdown'
Plugin 'jtratner/vim-flavored-markdown'
Plugin 'vim-scripts/octave.vim--'
Plugin 'vim-perl/vim-perl'
Plugin 'rust-lang/rust.vim'
" Plugin 'andersoncustodio/vim-tmux'
Plugin 'vim-scripts/Logcat-syntax-highlighter'

" ==== Other ==========================

Plugin 'L9'
Plugin 'tpope/vim-surround'
Plugin 'mileszs/ack.vim'
Plugin 'terryma/vim-expand-region'

Plugin 'scrooloose/nerdtree'
Plugin 'tpope/vim-vinegar'
Plugin 'vim-scripts/restore_view.vim'
Plugin 'tpope/vim-repeat'
Plugin 'tpope/vim-endwise'
Plugin 'tpope/vim-unimpaired'
Plugin 'Raimondi/delimitMate'
Plugin 'justincampbell/vim-eighties'

Plugin 'mhinz/vim-startify'

Plugin 'xuhdev/SingleCompile'
Plugin 'vim-scripts/Improved-AnsiEsc'
" Plugin 'vim-scripts/genutils'
" Plugin 'idbrii/vim-perforce'

" === C# and Uniy =====================
Plugin 'OmniSharp/omnisharp-vim'
Plugin 'OrangeT/vim-csharp'

" === Java Script and HTML =====================
Plugin 'pangloss/vim-javascript'
Plugin 'moll/vim-node'

" === Ruby ================================
Plugin 'tpope/vim-rails'
Plugin 'vim-ruby/vim-ruby'


"===== Themes =========================
" Plugin 'chriskempson/base16-vim' ==== still prefer molokai
Plugin 'tomasr/molokai'
Plugin 'sjl/badwolf'
Plugin 'jordwalke/flatlandia'
Plugin 'morhetz/gruvbox'
Plugin 'flazz/vim-colorschemes'
" Plugin 'benjaminwhite/Benokai'


" ==== Obsolete  =====================

" Plugin 'mkitt/tabline.vim'      " Buildin into airline
" Plugin 'bling/vim-bufferline'   " Deprecated since tabline buffer support
" Plugin 'Lokaltog/vim-powerline' " Using airline

call vundle#end()
filetype plugin indent on

" }}}


" =========================================================================
" Colors (should precede colorscheme command)
" =========================================================================
" {{{

" Show whitespace
" autocmd ColorScheme * highlight ExtraWhitespace ctermbg=124 guibg=red
" autocmd InsertLeave * match ExtraWhitespace /\s\+$/
" highlight Pmenu ctermfg=1 ctermbg=4 guibg=grey30

" }}}

" =========================================================================
" Settings
" =========================================================================
" {{{

" Eighties settings
 let g:eighties_bufname_additional_patterns = ['Tagbar']

" Rust completion
let g:ycm_rust_src_path = "/Users/demelev/Projects/Rust/rustc-1.5.0/src"

" Encoding
set termencoding=utf-8
set fileencodings=utf-8,cp1251
set encoding=utf-8

" Set spell for ru and eng
set spelllang=en,ru

" Scheme
" set background=dark
let g:rehash256 = 1
colorscheme molokai
" let base16colorspace=256
" colorscheme base16-default
" colorscheme base16-tomorrow

" Cursor free positioning
if g:bully_dev == "eugene"
    set virtualedit=all
endif

" Highlight syntax
syntax on

" No highlighting limit
"set synmaxcol=0

" Visual
set number         " Show line numbers
set relativenumber " Relative numbering for fast jumps
set novisualbell   " No blinking
set noerrorbells   " No noise
set laststatus=2   " 2 - always show status line
set ruler          " Show ruler
set showcmd        " Show command in last line
set shortmess+=I   " Remove splash on startup
set showbreak=»    " wrapping lines symbol
set nowrap         " Not wrap by default
set noshowmode     " Mode is in airline, no need stock one
set ttyfast        " Term is fast

set formatoptions-=t " Don't wrap while typing
set cmdwinheight=16  " Command-line window
set vb t_vb=

if bully_dev == "demelev"
    "set statusline=%<%f%h%m%r%=format=%{&fileformat}\ file=%{&fileencoding}\ enc=%{&encoding}\ %b\ 0x%B\ %l,%c%V\ %P
endif


" Unprintable
set nolist
set listchars=tab:‣\ ,trail:·,extends:>,eol:¶,precedes:·

" Buffers ans splits
set hidden " allow hidden buffers
set splitbelow " new splits go down
set splitright " and right


" place a dollar sign when in change mode to indicate end
if g:bully_dev == "eugene"
    set cpoptions+=$
endif

" Update time (def 4000)
set updatetime=750

" Keystrokes timeout
set timeoutlen=1000

if g:os == "Linux" || g:os == "Darwin"
    let g:dev_temp='/tmp'
elseif g:os == "Windows"
    if exists("$VIM_TMP")
        let g:dev_temp=$VIM_TMP
    else
       let g:dev_temp=$TMP
    endif
endif

" Backups "Risky but fast
exec ":set directory=".g:dev_temp

set backup
set writebackup
" set backupskip=/tmp/*
exec ":set backupdir=".g:dev_temp

set undofile
exec ":set undodir=".g:dev_temp


" History depth
set history=256
set undolevels=256

" Search

set hlsearch     " Highlight search results
set ignorecase   " no sensitive to case
set incsearch    " enable incremental search
set smartcase    " When meet uppercase -> sensitive

" Tabs ans indentation
set tabstop=4       " Tab size
set softtabstop=4   " Tab size in inset mode
set shiftwidth=4    " <> shift size
set expandtab       " Tabs to spaces
set smarttab        " Consolidated editing
set smartindent     " When starting new line repeat indentation
set autoindent

set wildmenu
set wildmode=full
set wildignore=*.o,*.obj,*~
set wildcharm=<Tab>

" Vim's default completion
set complete+=k
set complete+=kspell
set completeopt=menu,menuone,longest
let g:omnicomplete_fetch_documentation=1

" Folding
set foldenable
set foldmethod=syntax
set foldlevel=100       " Unfold on start
set foldopen=block,hor,mark,percent,quickfix,tag

" Russian layout
set keymap=russian-jcukenwin    " C-^ to switch
set iminsert=0                  " insert mode default en
set imsearch=0                  " search mode default en

" Vim modeline enabled and has 5 lines
set modeline
set modelines=5

" View options
set viewoptions=cursor,options,folds,slash,unix

" System default for mappings is now the ',' character
if g:bully_dev != "demelev"
    let mapleader = ","
endif

" Setup GVIM separately
if has("gui_running")

    if g:os == "Darwin"
        if bully_dev == "demelev"
            set guifont=DejaVu\ Sans\ Mono\ for\ Powerline:h14
            "set guifont=Anonymice\ Powerline:h14
        else
            set guifont=PragmataPro:h14
        endif
    elseif g:os == "Linux"
        if bully_dev == "demelev"
            set guifont=DejaVu\ Sans\ Mono\ for\ Powerline\ 14
            set guifont=Anonymice\ Powerline:h14
        else
            set guifont=PragmataProMono\ 12
        endif
    elseif g:os == "Windows"
        if bully_dev == "demelev"
            set guifont=DejaVu\ Sans\ Mono\ for\ Powerline:h12
        elseif bully_dev == "dstavila"
            set guifont=PragmataPro:h12
        else
            set guifont=PragmataPro:h14
        endif
    endif

    " Set default size for GVIM
    set lines=60 columns=200

    " Setup GVIM look, hide useless bars and stuff
    set guioptions-=T
    set guioptions-=m
    set guioptions-=l
    set guioptions-=L
    set guioptions-=r
    set guioptions-=b

   " Cursor settings
   " TODO: cursor play with blinking
    if g:bully_dev == "eugene"
        highlight Cursor guifg=black guibg=white
        highlight iCursor guifg=black guibg=red
        set guicursor=n-v-c:block-Cursor
        set guicursor+=i:ver100-iCursor
        set guicursor+=n-v:blinkon0
        set guicursor+=i-c:blinkwait10
    endif

    if g:os == "Linux"
   " Bash in gvim will understand my aliases
   set shell=/bin/bash\ --login
   endif

else

   " enable mouse in terminals
   set mouse=a

   " Poor man's terminal spelling colors
   highlight clear SpellBad
   highlight SpellBad term=standout ctermfg=1 term=underline cterm=underline
   highlight clear SpellCap
   highlight SpellCap term=underline cterm=underline
   highlight clear SpellRare
   highlight SpellRare term=underline cterm=underline
   highlight clear SpellLocal
   highlight SpellLocal term=underline cterm=underline

   " Italic comments on Linux (tmux must support)
   if g:os == "Linux" && g:bully_dev == "eugene"
       highlight Comment cterm=italic
   endif


endif

if g:os == "Cygwin" || g:os == "Windows"
    " Windows cygwin fix backspac
    set backspace=indent,eol,start
endif

" }}}

" =========================================================================
" Plugin settings
" =========================================================================
" {{{

" == openbrowser ===============
let g:openbrowser_default_search = 'unity3d'
let g:openbrowser_search_engines = extend(
    \get(g:, 'openbrowser_search_engines', {}),
    \{
    \   'unity3d' : 'http://docs.unity3d.com/ScriptReference/30_search.html?q={query}'
    \})

" == startify ===============
let g:startify_bookmarks = ['~/.vimrc','~/.zshrc','~/nfo/commands.txt',]
if g:os != "Windows"
    let g:startify_custom_header =
                \ map(split(system('fortune | cowsay -W 60'), '\n') , '"   ". v:val') + ['','']
endif
let g:startify_change_to_dir = 0
let g:startify_files_number = 8

" === Clang Format ==
"  "AllowShortIfStatementsOnASingleLine" : "true",
"  "AlwaysBreakTemplateDeclarations" : "true",
" Now uses ~/.clang-format file, no need to define settings here

" == gundo/undotree ==

" Make bigger
let g:undotree_SplitWidth = 40

" == restore_view===

" let g:skipview_files = ['*\.vim']

" == session ==

let g:session_directory = "~/.vim/session"
let g:session_autoload = "no"
let g:session_autosave = "no"
let g:session_command_aliases = 1

" == Indent guides ==

let g:indent_guides_start_level = 2
let g:indent_guides_guide_size = 1
let g:indent_guides_auto_colors = 0
let g:indent_guides_enable_on_vim_startup = 0

autocmd VimEnter,Colorscheme * :hi IndentGuidesOdd guibg=#31332B ctermbg=235
autocmd VimEnter,Colorscheme * :hi IndentGuidesEven guibg=#2E2F29 ctermbg=235

" == ProtoDef ==

let g:protodefprotogetter = '/home/randy/.vim/bundle/vim-protodef/pullproto.pl'

" == Airline / Powerline  ==

" bufferline
" let g:bufferline_modified = '*'
" let g:bufferline_echo = 0
" let g:Powerline_symbols="fancy"

let g:airline_theme='powerlineish'
let g:airline_powerline_fonts = 1
let g:airline#extensions#tabline#buffer_nr_show = 1
let g:airline#extensions#tabline#enabled = 1
let g:airline#extensions#tabline#fnamemod=':t'
let g:airline#extensions#tabline#show_buffers = 1

" == Syntastic ==

let g:syntastic_error_symbol="✖"
let g:syntastic_warning_symbol="⚠"
let g:syntastic_cs_checkers = ['syntax', 'semantic', 'issues']
set cmdheight=2

" == Unite ==

let g:unite_enable_start_insert = 1
let g:unite_split_rule = "botright"
let g:unite_force_overwrite_statusline = 0
let g:unite_winheight = 10
let g:unite_candidate_icon="▷"
let g:unite_source_rec_async_command= ['ag', '--follow', '--nocolor', '--nogroup', '--hidden', '-g', '']
let g:unite_source_history_yank_enable = 1

call unite#filters#matcher_default#use(['matcher_fuzzy'])
call unite#filters#sorter_default#use(['sorter_rank'])
call unite#custom_source('file_rec,file_rec/async,file_mru,file,buffer,grep',
\ 'ignore_pattern', join([
\ '.class$', '\.jar$',
\ '\.jpg$', '\.jpeg$', '\.bmp$', '\.png$', '\.gif$',
\ '\.o$', '\.so$', '\.lo$', '\.lib$', '\.out$', '\.obj$', 
\ '\.zip$', '\.tar\.gz$', '\.tar\.bz2$', '\.rar$', '\.tar\.xz$',
\ '\.ac$', '\.cache$', '\.0$', '\.meta$',
\ '\.anim$', '\.controller$'
\ ], '\|'))

" == You complete me ==

let g:ycm_global_ycm_extra_conf = '~/.ycm_extra_conf.py'
let g:ycm_auto_trigger = 1

" let g:ycm_key_invoke_completion = '<c-Tab>'
let g:ycm_key_list_select_completion = ['<tab>', '<up>']
"let g:ycm_key_list_previous_completion = ['<s-tab>']
"
let g:ycm_extra_conf_globlist = ['~/rdev/cpp/*']

" Disable for latex
let g:ycm_filetype_blacklist = {
      \ 'notes' : 1,
      \ 'markdown' : 1,
      \ 'text' : 1,
      \ 'tex' : 1,
      \}

" === LaTeX Box =====================
let g:LatexBox_viewer = 'zathura'
let g:LatexBox_latexmk_options = '-pvc -pdflatex="pdflatex -shell-escape"'

" == Ultisnips ==
let g:UltiSnipsListSnippets="<c-;>"
let g:UltiSnipsExpandTrigger="<c-l>"
let g:UltiSnipsJumpForwardTrigger="<c-j>"
let g:UltiSnipsJumpBackwardTrigger="<c-k>"

let g:UltiSnipsEditSplit="vertical"

" == Ack ===

let g:ackprg = 'ag --nogroup --nocolor --column'

" == Todo plugin ===
" todo.vim default highlight groups, feel free to override as wanted
hi link TodoTitle Title
hi link TodoTitleMark Normal
hi link TodoItem Special
hi link TodoItemAdditionalText Comment
hi link TodoItemCheckBox Identifier
hi link TodoItemDone Ignore
hi link TodoComment Comment

" define like this to enable explicit comments
" comments then start with //
let g:TodoExplicitCommentsEnabled = 1

" == OmniSharp ===
let g:Omnisharp_start_server = 0
let g:Omnisharp_stop_server  = 0
let g:OmniSharp_host="http://localhost:20001"
let g:ycm_csharp_server_port = 20001
let g:OmniSharp_timeout = 1
let g:OmniSharp_server_type = 'v1'
let g:OmniSharp_server_type = 'roslyn'

"if g:bully_dev == "dstavila"
    "let g:OmniSharp_selector_ui = "ctrlp"
"else 
    let g:OmniSharp_selector_ui = "unite"
"endif

" === Buffergator ===
let g:buffergator_suppress_keymaps = 1

" == NERD Tree ======
let NERDTreeWinPos='right'

" == Signify ======
" only use for hg for now 
let g:signify_vcs_list = [ 'hg' ]

" == Rust completion ======
let g:ycm_rust_src_path = "/Users/eugene/.cargo/sources/rustc-1.5.0/src"


" === Conque Settings =================
"let g:ConqueTerm_FastMode = 1
"let g:ConqueTerm_ReadUnfocused = 1
"let g:ConqueTerm_InsertOnEnter = 1
"let g:ConqueTerm_PromptRegex = '^-->'
"let g:ConqueTerm_TERM = 'rxvt-unicode-256color'
"let g:ConqueTerm_CloseOnEnd = 1
"let g:ConqueTerm_SendFunctionKeys = 1
"let g:ConqueTerm_Color = 1 "disable colors
"
" }}}

" =========================================================================
" Helper functions
" =========================================================================
" {{{

function! Enable80CharsLimit()
   set colorcolumn=80
   set textwidth=80
   " set formatoptions=cqt
   " set wrapmargin=0
   highlight ColorColumn ctermbg=235 guibg=#2c2d27
   highlight CursorLine ctermbg=235 guibg=#2c2d27
   highlight CursorColumn ctermbg=235 guibg=#2c2d27
   let &colorcolumn=join(range(81,999),",")
endfunction


" Translator with sdcv
function! TRANSLATE()
   let  a=getline('.')
   let co=col('.')-1
   let starts=strridx(a," ",co)
   let ends = stridx(a," ",co)
   if ends==-1
       let ends=strlen(a)
   endif
   let res = strpart(a,starts+1,ends-starts)
   let cmds = "sdcv -n --color --utf8-output " . res
   let out = system(cmds)
   echo out
endfunction

function! KbdLayoutState()
   if &iminsert == 0
      return 'en'
   else
      return 'ru'
   endif
endfunction

call airline#parts#define_function('bikeystat', 'KbdLayoutState')
let g:airline_section_a = airline#section#create(['mode', ' [', 'bikeystat', ']'])

function! FindProjectRoot(lookFor)
    let pathMaker='%:p'
    while(len(expand(pathMaker))>len(expand(pathMaker.':h')))
        let pathMaker=pathMaker.':h'
        let fileToCheck=expand(pathMaker).'/'.a:lookFor
        if filereadable(fileToCheck)||isdirectory(fileToCheck)
            return expand(pathMaker).'/'.a:lookFor
        endif
    endwhile
    return 0
endfunction

" Build the ctrlp function, using projectroot to define the
" working directory.
function! Unite_ctrlp()
  execute ':Unite  -buffer-name=files -start-insert buffer file_rec/async:'.fnameescape(FindProjectRoot("Assets")).'/'
endfunction

function! s:unite_settings()
  " Play nice with supertab
  let b:SuperTabDisabled=1
  " Enable navigation with control-j and control-k in insert mode
  imap <buffer> <C-j>   <Plug>(unite_select_next_line)
  imap <buffer> <C-k>   <Plug>(unite_select_previous_line)
endfunction

" Quick Fix window on/off
let g:quickfix_is_open = 0
function! s:QuickfixToggle()
    if g:quickfix_is_open
        cclose
        let g:quickfix_is_open = 0
        execute g:quickfix_return_to_window . "wincmd w"
    else
        let g:quickfix_return_to_window = winnr()
        copen
        let g:quickfix_is_open = 1
    endif
endfunction

" Open url in browser
function! OpenURL(url)
    if g:os == "Darwin"
        " TODO: Write cross platform solution
    elseif g:os == "Linux"
        exe "silent !chromium \"".a:url."\""
    else " Win
        " TODO: Write cross platform solution
    endif
    redraw!
endfunction
command! -nargs=1 OpenURL :call OpenURL(<q-args>)

"}}}

let g:ctrlp_extensions = ['autoignore']
"let g:ctrlp_root_markers = ['_vimroot']
"let g:ctrlp_working_path_mode = "r"
"let g:ctrlp_custom_ignore = '\v[\/]\.(git|hg|svn)$|.*\.(meta|scene|anim|prefab)$'

" =========================================================================
" Helper menus
" =========================================================================
" {{{

" Encodings
menu Encoding.UTF-8          :e ++enc=utf-8
menu Encoding.Unicode        :e ++enc=unicode
menu Encoding.UCS-2          :e ++enc=ucs-2le<CR>
menu Encoding.UCS-4          :e ++enc=ucs-4
menu Encoding.KOI8-R         :e ++enc=koi8-r ++ff=unix <CR>
menu Encoding.KOI8-U         :e ++enc=koi8-u ++ff=unix <CR>
menu Encoding.CP1251         :e ++enc=cp1251
menu Encoding.IBM-855        :e ++enc=ibm855
menu Encoding.IBM-866        :e ++enc=ibm866 ++ff=dos <CR>
menu Encoding.ISO-8859-5     :e ++enc=iso-8859-5
menu Encoding.Latin-1        :e ++enc=latin1

" File formats
menu FileFormat.UNIX         :e ++ff=unix
menu FileFormat.DOS          :e ++ff=dos
menu FileFormat.Mac          :e ++ff=mac

" }}}

" =========================================================================
" Keyboard mappings
" =========================================================================
" {{{
"

if g:os == "Darwin"
    nmap µ <A-m>
    nmap ∫ <A-b>
    map \ <leader>
endif



" == Function keys ==============

" F2 for quickfix
nmap <silent> <F2>  :call <SID>QuickfixToggle()<cr>
imap <silent> <F2>  <c-o>:call <SID>QuickfixToggle()<cr>

" Translator function
nmap <silent> <F3>  :call TRANSLATE()<cr>
imap <silent> <F3>  <c-o>:call TRANSLATE()<cr>

if g:bully_dev == "demelev"

    " Save file
    nmap <F4> :w!<CR>
    imap <F4> <Esc>:w!<CR>
    vmap <F4> <Esc>:w!<CR>

    " Quit vim
    nmap <F5> :q<CR>
    imap <F5> <Esc>:q<CR>
    vmap <F5> <Esc>:q<CR>

elseif g:bully_dev == "eugene"

    " Single compile binding
    nmap <silent> <F5> <ESC>:SCCompile<CR>
    nmap <silent> <F6> <ESC>:SCCompileRun<CR>

endif

" Toggle spelling with the F7 key
nmap <silent> <F7> <ESC>:setlocal spell!<CR>
imap <silent> <F7> <c-o>:setlocal spell!<CR>

" Toggle keyboard layout
imap <silent> <F9> <C-^>

" Toggle unprintable <F10>
nmap <silent> <F10> <ESC>:set list!<CR>
imap <silent> <F10> <c-o>:set list!<CR>

nmap <silent>  <F11> :emenu Encoding.<Tab><Tab>
nmap <silent> <S-F11> :emenu FileFormat.<Tab><Tab>

nmap <silent> <F12> :NERDTreeToggle<CR>

" == Leader mappings =============

" CtrlP maps
" TODO: clear it
map <A-b> :CtrlPBuffer<cr>
map <A-m> :CtrlPBufTag<cr>
map <c-Tab> :tabn<cr>

" OmniSharp bindings TODO: compare with Eugene's - CS only!
nnoremap <leader>fi :OmniSharpFindImplementations<cr>
nnoremap <leader>ft :OmniSharpFindType<cr>
nnoremap <leader>fs :OmniSharpFindSymbol<cr>
nnoremap <leader>fu :OmniSharpFindUsages<cr>
nnoremap <leader>fm :OmniSharpFindMembersInBuffer<cr>
nnoremap <leader><space> :OmniSharpFindMembersInBuffer<cr>

" cursor can be anywhere on the line containing an issue for this one
nnoremap <leader>x  :OmniSharpFixIssue<cr>
nnoremap <leader>fx :OmniSharpFixUsings<cr>
nnoremap <leader>tt :OmniSharpTypeLookup<cr>
nnoremap <leader>dc :OmniSharpDocumentation<cr>
nnoremap <leader>gd :OmniSharpGotoDefinition<cr>

" Session workflow
nmap <leader>so :OpenSession<space>
nmap <leader>ss :SaveSession<space>
nmap <leader>sd :DeleteSession<CR>
nmap <leader>sc :CloseSession<CR>


"TODO: clear there.
vmap <leader>wr :WrapWithRegion<cr>
vmap <leader>ifed :WrapWithIf "UNITY_EDITOR"<cr>
vmap <leader>ifedd :WrapWithIf 'UNITY_EDITOR \|\| DEVELOPMENT'<cr>


nmap <leader>wr :WrapWithRegion<cr>
nmap <leader>ifed :WrapWithIf "UNITY_EDITOR"<cr>

" Toggle things
" nmap <leader>1 :GundoToggle<CR>
nmap <leader>1 :UndotreeToggle<CR>
set pastetoggle=<leader>2
nmap <leader>3 :TlistToggle<CR>
nmap <leader>4 :TagbarToggle<CR>
nmap <leader>5 :NERDTreeToggle<CR>
nmap <leader>6 :BuffergatorToggle<CR>
"nmap <silent> <leader>6 :ConqueTermSplit bash<CR><Esc>:setlocal nolist<CR>a

" Make p in Visual mode replace the selected text with the \" register.
vmap p <Esc>:let current_reg = @"<CR>gvdi<C-R>=current_reg<CR><Esc>

" Indentation changes, but visual stays
vnoremap > ><CR>gv
vnoremap < <<CR>gv

" move current line up/down one
nmap <c-j> ddp
nmap <c-k> ddkP

" move visual block up/down one
vmap <c-j> dp'[V']
vmap <c-k> dkP'[V']

" Duplications
" TODO: prevent register wipe
vmap <silent> <leader>= yP
nmap <silent> <leader>= YP

" move stuff to the right of cursor to next line
nmap <silent> <leader><CR> i<CR><ESC>k$

" cd to the directory containing the file in the buffer
nmap <silent> <leader>cd :lcd %:h<CR>

" make directory
nmap <silent> <leader>md :!mkdir -p %:p:h<CR>

" When search done : ,n to remove highlight
nmap <silent> <leader>n :nohls<CR>

" FSwitch mappings
nmap <silent> <leader>of :FSHere<CR>
nmap <silent> <leader>ol :FSRight<CR>
nmap <silent> <leader>oL :FSSplitRight<CR>
nmap <silent> <leader>oh :FSLeft<CR>
nmap <silent> <leader>oH :FSSplitLeft<CR>
nmap <silent> <leader>ok :FSAbove<CR>
nmap <silent> <leader>oK :FSSplitAbove<CR>
nmap <silent> <leader>oj :FSBelow<CR>
nmap <silent> <leader>oJ :FSSplitBelow<CR>

" Alright... let's try this out
imap jj <esc>

" Line wrap toggle
nmap <silent> <leader>w :set invwrap<CR>:set wrap?<CR>

" Buffers

" == Space mappings ==

" Openbrowser maps
"nmap <leader>qu <Plug>(openbrowser-search)
nmap <space>sg :OpenBrowserSearch -google <c-r>=expand("<cword>")<cr><cr>
nmap <space>su :OpenBrowserSearch -unity3d <c-r>=expand("<cword>")<cr><cr>
nmap <space>ag :OpenBrowserSearch -google 
nmap <space>au :OpenBrowserSearch -unity3d 

" Faster command access
nmap <silent> <space> <NOP>
nmap <space>;  :
nmap <space><space>  :
nmap <silent> <space>w  :w<CR>
nmap <silent> <space>q  :q<CR>
nmap <silent> <space>]  :bn<CR>
nmap <silent> <space>[  :bp<CR>
nmap <silent> <space>c  :bd<CR>

" Remove trailing whitespaces
nmap <silent> <leader>rtw :%s/\s\+$//e<CR>:nohl<CR>
nmap <silent> <leader>rrt :%s/\t/    /g<CR>:nohl<CR>

" Copy paste to + register
nmap <silent> <space>y "+yy
vmap <silent> <space>y "+y
nmap <silent> <space>p "+p
nmap <silent> <space>pp "*p
nmap <silent> <space>P "+P
nmap <silent> <space>PP "*P


" Search the current file for the word under the cursor and display matches
nmap <silent> <leader>gw :vimgrep /<C-r><C-w>/ %<CR>:ccl<CR>:cwin<CR><C-W>J:nohls<CR>

" Edit the vimrc file
nmap <silent> <leader>ev :e $MYVIMRC<CR>
nmap <silent> <leader>sv :so $MYVIMRC<CR>

" Yank to end (like D and C)
nmap Y y$

" When entering command, press %% to quickly insert current path
cmap %% <C-R>=expand('%:h').'/'<cr>

" Mapping ,, to fast switch between buffers
nmap <silent> <leader><leader><space> <c-^>

" Tab in normal mode is useless - use it to %
nmap <Tab> %
vmap <Tab> %

" Ack on ,a
nmap <leader>a :Ack<space>

vmap v <Plug>(expand_region_expand)
vmap <c-v> <Plug>(expand_region_shrink)

" Search
nnoremap gb :OpenURL <cfile><CR>
nnoremap gA :OpenURL http://www.answers.com/<cword><CR>
nnoremap gG :OpenURL http://www.google.com/search?q=<cword><CR>
nnoremap gW :OpenURL http://en.wikipedia.org/wiki/Special:Search?search=<cword><CR>

" Folds
" Mappings to easily toggle fold levels
nnoremap z0 :set foldlevel=0<cr>
nnoremap z1 :set foldlevel=1<cr>
nnoremap z2 :set foldlevel=2<cr>
nnoremap z3 :set foldlevel=3<cr>
nnoremap z4 :set foldlevel=4<cr>
nnoremap z5 :set foldlevel=5<cr>


" === YCM =====
nmap <leader>yy :YcmForceCompileAndDiagnostics<cr>
nmap <leader>yg :YcmCompleter GoToDefinitionElseDeclaration<cr>
nmap <leader>yd :YcmCompleter GoToDefinition<cr>
nmap <leader>yc :YcmCompleter GoToDeclaration<cr>
nmap <leader>yt :YcmCompleter GetType<cr>


" == Unite =====
let g:unite_source_history_yank_enable = 1
call unite#filters#matcher_default#use(['matcher_fuzzy'])

nnoremap <leader>uf :<C-u>Unite -no-split -buffer-name=files   -start-insert file_rec/async <cr>

nnoremap <leader>ut :<C-u>Unite -no-split -buffer-name=files   -start-insert file<cr>
nnoremap <leader>ur :<C-u>Unite -no-split -buffer-name=mru     -start-insert file_mru<cr>
nnoremap <leader>uo :<C-u>Unite -no-split -buffer-name=outline -start-insert outline<cr>
nnoremap <leader>uy :<C-u>Unite -no-split -buffer-name=yank    history/yank<cr>
nnoremap <leader>ub :<C-u>Unite -buffer-name=buffer  buffer<cr>
nnoremap <leader>ul :<C-u>Unite -buffer-name=lines  line<cr>
nnoremap <leader>um :<C-u>Unite -buffer-name=bookmarks  bookmark<cr>
nnoremap <leader>uc :<C-u>Unite colorscheme<cr>
nnoremap <leader>uh :<C-u>Unite resume<cr>

" Custom mappings for the unite buffer
autocmd FileType unite call s:unite_settings()
function! s:unite_settings()
  " Play nice with supertab
  let b:SuperTabDisabled=1
  " Enable navigation with control-j and control-k in insert mode
  imap <buffer> <C-j>   <Plug>(unite_select_next_line)
  imap <buffer> <C-k>   <Plug>(unite_select_previous_line)
endfunction

" Tab in normal mode is useless - use it to %
nmap <Tab> %
vmap <Tab> %

" Ack on ,a
nmap <leader>a :Ack<space>

if bully_dev != "demelev"
    " == Fugitive =======
    noremap <leader>gd :Gdiff<CR>
    noremap <leader>gc :Gcommit<CR>
    noremap <leader>gs :Gstatus<CR>
    noremap <leader>gw :Gwrite<CR>
    noremap <leader>gb :Gblame<CR>
endif

vmap v <Plug>(expand_region_expand)
vmap <c-v> <Plug>(expand_region_shrink)
" }}}


" =========================================================================
" Filetype speicific
" =========================================================================
" {{{

function! SetupLatex()

    call Enable80CharsLimit()

    " Add triggers to ycm for LaTeX-Box autocompletion
    let g:ycm_semantic_triggers = {
    \  'tex'  : ['{'],
    \ }

endfunction

function! SetupCpp()

    call Enable80CharsLimit()
    :IndentGuidesEnable

    " Clang formar
    nnoremap <buffer>,cf :<C-u>ClangFormat<CR>
    vnoremap <buffer>,cf :ClangFormat<CR>

    call SingleCompile#ChooseCompiler('cpp', 'clang')

    nmap <buffer> <F5> :SCCompileAF -O0 -ggdb -std=c++11 -stdlib=libc++ -lc++abi -lpthread<cr>
    nmap <buffer> <F6> :SCCompileRunAF -O0 -ggdb -std=c++11 -stdlib=libc++ -lc++abi -lpthread<cr>

    " FSwitch mappings
    nmap <silent> <leader>of :FSHere<CR>
    nmap <silent> <leader>ol :FSRight<CR>
    nmap <silent> <leader>oL :FSSplitRight<CR>
    nmap <silent> <leader>oh :FSLeft<CR>
    nmap <silent> <leader>oH :FSSplitLeft<CR>
    nmap <silent> <leader>ok :FSAbove<CR>
    nmap <silent> <leader>oK :FSSplitAbove<CR>
    nmap <silent> <leader>oj :FSBelow<CR>
    nmap <silent> <leader>oJ :FSSplitBelow<CR>


endfunction

function! SetupCs()

    if g:bully_dev == "eugene"
        call Enable80CharsLimit()
        :IndentGuidesEnable
    endif

    " == Unite ===
    nnoremap <leader>uf :call Unite_ctrlp()<cr>

    " == Omnisharp ===
    nnoremap <leader>sg :OmniSharpGotoDefinition<cr>
    nnoremap <leader>si :OmniSharpFindImplementations<cr>
    nnoremap <leader>st :OmniSharpFindType<cr>
    nnoremap <leader>ss :OmniSharpFindSymbol<cr>
    nnoremap <leader>su :OmniSharpFindUsages<cr>
    nnoremap <leader>sm :OmniSharpFindMembers<cr>
    nnoremap <leader>sx  :OmniSharpFixIssue<cr>
    nnoremap <leader>sxu :OmniSharpFixUsings<cr>
    nnoremap <leader>st :OmniSharpTypeLookup<cr>
    nnoremap <leader>sd :OmniSharpDocumentation<cr>
    nnoremap <leader>sk :OmniSharpNavigateUp<cr>
    nnoremap <leader>sj :OmniSharpNavigateDown<cr>
    nnoremap <leader>sl :OmniSharpReloadSolution<cr>
    nnoremap <leader>sf :OmniSharpCodeFormat<cr>
    nnoremap <leader>sa :OmniSharpAddToProject<cr>

    " Contextual code actions (requires CtrlP or unite.vim)
    nnoremap <leader><space> :OmniSharpGetCodeActions<cr>
    " Run code actions with text selected in visual mode to extract method
    vnoremap <leader><space> :call OmniSharp#GetCodeActions('visual')<cr>

    " rename with dialog
    nnoremap <leader>sr :OmniSharpRename<cr>
    " rename without dialog - with cursor on the symbol to rename... ':Rename newname'
    command! -nargs=1 Rename :call OmniSharp#RenameTo("<args>")

    " (Experimental - uses vim-dispatch or vimproc plugin) - Start the omnisharp server for the current solution
    nnoremap <leader>ss :OmniSharpStartServer<cr>
    nnoremap <leader>sp :OmniSharpStopServer<cr>

    " Add syntax highlighting for types and interfaces
    nnoremap <leader>sh :OmniSharpHighlightTypes<cr>

endfunction

function! SetupPython()

    call Enable80CharsLimit()
    :IndentGuidesEnable

endfunction

" =========================================================================
" Autos
" =========================================================================
" {{{

" Custom mappings for the unite buffer
autocmd FileType unite call s:unite_settings()

" Working in 80 cols
autocmd BufNewFile,BufRead .vimrc,.zshrc call Enable80CharsLimit()


" IDEs
autocmd BufNewFile,BufRead *.cpp,*.h,*.c call SetupCpp()
autocmd BufNewFile,BufRead *.py  call SetupPython()
autocmd BufNewFile,BufRead *.tex call SetupLatex()
autocmd BufNewFile,BufRead *.cs call SetupCs()

" }}}


" TODO: Insert g:bully_dev here?
if filereadable(expand("$VIM/vimfiles/demelev/helpers.vim"))
    source $VIM/vimfiles/demelev/helpers.vim
endif

" =============================================================================
" Tab visual 
" =============================================================================
" {{{
" Задаем собственные функции для назначения имен заголовкам табов -->
    function! MyTabLine()
        let tabline = ''

        " Формируем tabline для каждой вкладки -->
            for i in range(tabpagenr('$'))
                " Подсвечиваем заголовок выбранной в данный момент вкладки.
                if i + 1 == tabpagenr()
                    let tabline .= '%#TabLineSel#'
                else
                    let tabline .= '%#TabLine#'
                endif

                " Устанавливаем номер вкладки
                let tabline .= '%' . (i + 1) . 'T'

                " Получаем имя вкладки
                let tabline .= ' %{MyTabLabel(' . (i + 1) . ')} |'
            endfor
        " Формируем tabline для каждой вкладки <--

        " Заполняем лишнее пространство
        let tabline .= '%#TabLineFill#%T'

        " Выровненная по правому краю кнопка закрытия вкладки
        if tabpagenr('$') > 1
            let tabline .= '%=%#TabLine#%999XX'
        endif

        return tabline
    endfunction

    function! MyTabLabel(n)
        let label = ''
        let buflist = tabpagebuflist(a:n)

        " Имя файла и номер вкладки -->
            let label = substitute(bufname(buflist[tabpagewinnr(a:n) - 1]), '.*/', '', '')

            if label == ''
                let label = '[No Name]'
            endif

            let label .= ' (' . a:n . ')'
        " Имя файла и номер вкладки <--

        " Определяем, есть ли во вкладке хотя бы один
        " модифицированный буфер.
        " -->
            for i in range(len(buflist))
                if getbufvar(buflist[i], "&modified")
                    let label = '[+] ' . label
                    break
                endif
            endfor
        " <--

        return label
    endfunction

    function! MyGuiTabLabel()
        return '%{MyTabLabel(' . tabpagenr() . ')}'
    endfunction

    set tabline=%!MyTabLine()
    set guitablabel=%!MyGuiTabLabel()
" Задаем собственные функции для назначения имен заголовкам табов <--
"
" }}}


" ==============================================================================
" Appendix
" ==============================================================================
if bully_dev == "demelev"
    colorscheme Monokai_next
endif

" vim -b : edit binary using xxd-format!
augroup Binary
  au!
  au BufReadPre  *.bin let &bin=1
  au BufReadPost *.bin if &bin | %!xxd
  au BufReadPost *.bin set ft=xxd | endif
  au BufWritePre *.bin if &bin | %!xxd -r
  au BufWritePre *.bin endif
  au BufWritePost *.bin if &bin | %!xxd
  au BufWritePost *.bin set nomod | endif
augroup END

"Function for autoloading project's settings and highlight
function! On_session_loaded()
    if exists("g:project_settings_loaded")
        return
    endif

    if filereadable('.vim/project_settings.vim')
        exec "source ".expand('~/.vim/SimpleIDE/idecs.vim')
        let g:project_settings_loaded = 1
    endif
endfunction
autocmd SessionLoadPost * call On_session_loaded()

"
"function! PreviewWord()
    "exec ":ptjump ".expand("<cword>")
"endfunction
"
