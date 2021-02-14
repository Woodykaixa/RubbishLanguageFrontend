# RubbishLanguageFrontend
This is a compiler frontend for my toy language, RubbishLanguage(rblang). This frontend will generate CIL and depends on .Net Core

## The Language

### keywords

1. i64 - 64-bit integer
2. f64 - 64-bit float point
3. char - 16-bit character 
4. str - string
5. void
6. if 
7. else
8. elif - else if
9. loop - just like while in C/C++
10. and - python style condition operator &&
11. or - ||
12. not - !
13. func
14. return

## operators
|operator|precedence(the higher, the better)|
|:---:|:---:|
|=(assign)|1|
|or|2|
|and|3|
|==|4|
|<|5|
|<=|5|
|>|5|
|>=|5|
|+|6|
|-|6|
|*(multiplication)|7|
|/|7|
|%|7|
|not|8|
|address_of|8|

### attributes
any token starts with '@'

1. @entry - the entry of the program
2. @import - import a function

## Progress

Currently, the lexer is finished and the test is passed. But the lexer still has some problems, such as the line/column counter. And char type is not implemented. I think the lexer need more test cases.