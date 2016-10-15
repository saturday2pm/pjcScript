# pjcScript

쓰지 마셈

## 명세

`+, -, *, /, %, =` 기본 연산 사용 가능. 함수 호출 `func(param1, param2, ...)` 형태로 사용 가능.

`?` 연산도 있음.

`a ? b` 하면 `a` 값이 `null`이면 `b` 리턴하고 아니면 `a`값 씀.

변수 값은 외부에서 바인딩 가능하고, 함수는 바인딩해야만 쓸 수 있음. 바인딩되지 않은 변수는 대입하는 시점에서의 값으로 초기화해서 들고 있음.
스크립트의 결과 값은 항상 맨 마지막 표현식의 결과 값이 됨(따로 리턴 값이 없는 경우 null).

문장 끝에는 세미콜론

## 사용법

```C#
Interpreter interpreter;
int a = 3;
int b = -5;

interpreter.Bind("a", a);
interpreter.Bind("b", b);
interpreter.Bind("abs", Func<int, int>(Math.Abs));
int res = (int)interpreter.Exec("c = a + b; abs(c);");

Console.WriteLine(res); // 2
```