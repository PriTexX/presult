## What is this?

`PResult` is a library that provides you a better way of handling application errors with `Result` pattern.

Instead of throwing exceptions from methods you can now just return `Result<T>` type from them. It wraps your value or error and represents either `Ok` or `Err ` variant of returned value.

Also, `Result<T>` enforces you to handle both `Ok` and `Err` cases so you cant accidently forget to handle your exception.

## API

`Result<T>` exposes fluent api allowing you chain methods that can transform your initial value or error or continue your application flow not worrying of errors using `Then` method.

> Example:
> ```cs
> void Consumer()
> {
> 	var res = SomeMethod(false);
>	
>	var val = res
>		.Then<string>(okVal => "Received value")	
>		.Match(okVal => okVal, error => {
>			Console.WriteLine(error.Message) // Log error
>			return "No value received";
>		});
> 	
> 	Console.WriteLine(val); // "Received value"
> }
>
> Result<int> SomeMethod(bool flag)
> {
> 	if(flag)
> 	{
> 		return new Exception("Error");
> 	}
> 	
> 	return 1;
> }
> ```



## Async

Library also provides `AsyncResult<T>` type that represents asynchrounus operation that may return an error and async overloads of all sync methods.


> NOTE:
> 
> All async overloads return `AsyncResult<T>`
> You can directly `await` `AsyncResult<T>` to receive get `Result<T>` from it.

## LICENSE
[MIT](https://github.com/PriTexX/presult/blob/main/LICENSE)

