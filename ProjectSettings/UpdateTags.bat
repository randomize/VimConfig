echo sed -i .a "/%1/d" "tags"
echo rm 'tags.a'
echo ctags -a -f "tags" --c#-kinds=+cimnp --fields=+ianmzS --extra=+fq %2
echo open /Applications/MacVim.app --args --servername %3 --remote-send ":UpdateTypesFileOnly<CR>"
