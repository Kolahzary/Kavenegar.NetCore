# Kavenegar .NetCore SDK

# [Kavenegar RESTful API Document](http://kavenegar.com/rest.html)
If you need to future information about API document Please visit RESTful Document

## Installation
First of all, You need to make an account on Kavenegar from [Here](https://panel.kavenegar.com/Client/Membership/Register)
After that you just need to pick API-KEY up from [My Account](http://panel.kavenegar.com/Client/setting/index) section.
You can download the C# SDK [Here](https://github.com/Kolahzary/Kavenegar.NetCore/releases) or just pull it.
Anyway there is good tutorial about [Pull  request](http://gun.io/blog/how-to-github-fork-branch-and-pull-request/).

## Usage
Well, There is an example to Send SMS by C#.

```c#
try
{
	Kavenegar.KavenegarApi api = new Kavenegar.KavenegarApi("Your Api Key");
	var result = api.Send("SenderLine", "Your Receptor", "خدمات پیام کوتاه کاوه نگار");
	foreach (var r in result){
	  Console.Write("r.Messageid.ToString()");
  }
}
catch (Kavenegar.Exceptions.ApiException ex) 
{
	// در صورتی که خروجی وب سرویس 200 نباشد این خطارخ می دهد.
	Console.Write("Message : " + ex.Message);
}
catch (Kavenegar.Exceptions.HttpException ex) 
{
	// در زمانی که مشکلی در برقرای ارتباط با وب سرویس وجود داشته باشد این خطا رخ می دهد
	Console.Write("Message : " + ex.Message);
}
```

## Contribution
Bug fixes, docs, and enhancements welcome! Please let us know [support@kavenegar.com](mailto:support@kavenegar.com?Subject=SDK)

## راهنما

### معرفی سرویس کاوه نگار

کاوه نگار یک وب سرویس ارسال و دریافت پیامک و تماس صوتی است که به راحتی میتوانید از آن استفاده نمایید.

### ساخت حساب کاربری

اگر در وب سرویس کاوه نگار عضو نیستید میتوانید از [لینک عضویت](http://panel.kavenegar.com/client/membership/register) ثبت نام  و اکانت آزمایشی برای تست API دریافت نمایید.

### مستندات

برای مشاهده اطلاعات کامل مستندات [وب سرویس پیامک](http://kavenegar.com/وب-سرویس-پیامک.html)  به صفحه [مستندات وب سرویس](http://kavenegar.com/rest.html) مراجعه نمایید.

### راهنمای فارسی

در صورتی که مایل هستید راهنمای فارسی کیت توسعه کاوه نگار را مطالعه کنید به صفحه [کد ارسال پیامک](http://kavenegar.com/sdk.html) مراجعه نمایید.

### اطالاعات بیشتر
برای مطالعه بیشتر به صفحه معرفی
[وب سرویس اس ام اس ](http://kavenegar.com)
کاوه نگار
مراجعه نمایید .

 اگر در استفاده از کیت های سرویس کاوه نگار مشکلی یا پیشنهادی  داشتید ما را با یک Pull Request  یا  ارسال ایمیل به support@kavenegar.com  خوشحال کنید.
 
##
![http://kavenegar.com](http://kavenegar.com/public/images/logo.png)		

[http://kavenegar.com](http://kavenegar.com)	

Thanks to https://github.com/mberneti/Kavenegar.Core
