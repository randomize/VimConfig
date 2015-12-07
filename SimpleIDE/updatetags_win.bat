@sed -i .a "/%1/d" "tags"
@rem del tags.a
@ctags -a -f "tags" --c#-kinds=+cimnp --fields=+ianmzS --extra=+fq %2
@gvim --servername %3 --remote-send ":UpdateTypesFileOnly<CR>"
