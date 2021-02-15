# RubbishLanguageFrontend

This is a compiler frontend for my toy language, RubbishLanguage(rblang). This frontend will generate MSIL and depends on .Net Core

---

__Updated at 2021-02-14__

Alright, new plan. This project will interpret source code and generate MSIL, then execute it on CLR. It seems that the new API of AssemblyBuilder does not support saving the CIL to disk, so this project becomes an interpreter.
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
10. break
11. continue
12. and - python style condition operator &&
13. or - ||
14. not - !
15. func
16. return

## operators
|     operator      | precedence(the higher, the better) |
| :---------------: | :--------------------------------: |
|     =(assign)     |                 1                  |
|        or         |                 2                  |
|        and        |                 3                  |
|        ==         |                 4                  |
|         <         |                 5                  |
|        <=         |                 5                  |
|         >         |                 5                  |
|        >=         |                 5                  |
|         +         |                 6                  |
|         -         |                 6                  |
| *(multiplication) |                 7                  |
|         /         |                 7                  |
|         %         |                 7                  |
|        not        |                 8                  |
|    address_of     |                 8                  |

### attributes
any token starts with '@'

1. @entry - the entry of the program
2. @import - import a function

## Progress

__2021-02-05__

Currently, the lexer is finished and the test is passed. But the lexer still has some problems, such as the line/column counter. And char type is not implemented. I think the lexer need more test cases.

---

__2021-02-14__

Implemented a parser, which convert token stream to abstract syntax tree. But I forgot to handle loop statement

## Todo

- [x] Lexer
- [x] Parser
- [ ] Semantic analysis
- [ ] CodeEmitter
- [ ] Fix column counting problem for RbLexer
- [ ] Array types
- [ ] Dot operator(.) and struct type