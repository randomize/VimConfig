syn region Done start="^+" end="\ze\n\n\|^\ze-"
syn region NotDone start = "^-" end = "\ze\n\n\|^\ze+"
hi Done guifg=#00ff00
hi NotDone guifg=#ff0000
