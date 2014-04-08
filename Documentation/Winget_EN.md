# ScriptContext Class
C#, doesn't support global methods, so jQuery's `$` function can't be used as simply in Saltarelle as in Javascript. 

A simple expression like `$('#SomeElementId)` in Javascript corresponds to Saltarelle C# code `jQuery.Select("#SomeElementId")`.

As a workaround, *ScriptContext* class can be used:

```cs
public class ScriptContext
{
    [InlineCode("$({p})")]
    protected static jQueryObject J(object p);
    [InlineCode("$({p}, {context})")]
    protected static jQueryObject J(object p, object context);
}
```

As `$` is not a valid method name in C#, `J` is used instead. In subclasses of ScriptContext, jQuery.Select() function can be called briefly as `J()`.

```cs

public class SampleClass : ScriptContext
{
    public void SomeMethod()
    {
        J("#SomeElementId").AddClass("abc");
    }
}
```

# Widget Class

Serenity script user interface (*Serenity.UI*) layer's component classes (control), are based on a system that is similar to *jQuery UI*'s *Widget Factory*, but redesigned for for C#.

> You can find more information about jQuery UI widget system :
>
> http://learn.jquery.com/jquery-ui/widget-factory/
>
> http://msdn.microsoft.com/en-us/library/hh404085.aspx

Widget, is an object that is applied to an HTML element, and extends it with some behaviour. 

For example, IntegerEditor widget, when applied to an INPUT element, makes it easier to enter numbers in the input, and validates that the number entered is a correct integer.

Similarly, a Toolbar widget, when applied to a DIV element, turns it into a toolbar with tool buttons (here DIV acts as a placeholder).

## Widget Class Diagram
![Widget Class Diagram][1]

## A sample Widget

Let's build a widget that increases a DIV's font size, everytime it is clicked:

```cs
namespace MySamples
{
    public class MyCoolWidget : Widget
    {
        private int fontSize = 10;
        
        public MyCoolWidget(jQueryObject div)
            : base(div)
        {
            div.Click(e => {
                fontSize++;
                this.Element.Css("font-size", fontSize + "pt");
            });
        }
    }
}
```

```html
<div id="SomeDiv">Sample Text</div>
```

We can create this widget on an HTML element, like:

```cs
var div = jQuery.Select("#SomeDiv");
new MyCoolWidget(div);
```

## Widget Class Members
```cs
public abstract class Widget : ScriptContext
{
    private static int NextWidgetNumber = 0;
    protected string uniqueName;

    protected Widget(jQueryObject element);
    public virtual void Destroy();

    protected virtual void OnInit();
    protected virtual void AddCssClass();

    public jQueryObject Element { get; }
    public string WidgetName { get; }
}
```

### *Widget.Element* Property

Classes derived from Widget, can access the element, on which they are created on, by `Element` property.

```cs
public jQueryObject Element { get; }
```

This property has type of jQueryObject, and returns the element that is used when widget is created. In our sample, container DIV element is accessed as `this.Element` in the click handler. 

### HTML Element and Widget *CSS Class*

When a widget is created on an HTML element, it does some modifications to the element.

First, the HTML element gets a CSS class, based on type of the widget.

In our sample, `.s-MyCoolWidget` class is added to `DIV` with ID `#SomeDiv`.

Thus, after widget creation, DIV looks similar to this:

```html
<div id="SomeDiv" class="s-MyCoolWidget">Sample Text</div>
```

CSS sınıfı, widget sınıf adının başına `s-` getirilerek elde edilir. (Widget.AddCssClass metodu override edilerek bu isimlendirme değiştirilebilir)

### Widget CSS Sınıfı İle HTML elementinı Stillendirme

Bu CSS sınıfı sayesinde, dilersek HTML elementini, üzerinde oluşturulan widget tipine göre CSS tarafında stillendirebiliriz.

```css
.s-MyCoolWidget {
	background-color: red;
}
```

### HTML elementindan *jQuery.Data* İle Widget'a Erişim

Ayrıca elementimızın data özelliğine de üzerine eklenen widget ile ilgili bir bilgi eklendi. Chrome konsolunda bu veriyi aşağıdaki gibi görebiliriz:

```js
> $('#SomeDiv').data()

> Object { MySamples_MyCoolWidget: $MySamples_MyCoolWidget }
```

Yani, data özelliği üzerinden, bir elemente daha önce bağladığımız herhangi bir Widget'e ulaşmamız mümkün.

```cs
var myWidget = (MyCoolWidget)(J("#SomeDiv").GetDataValue('MySamples_MyCoolWidget'));
```

### WidgetExtensions.GetWidget Uzantı Metodu

Biraz uzun ve karışık gözüken bu kod parçası yerine, Serenity kısayolu kullanılabilir:

```cs
var myWidget = J("#SomeDiv").GetWidget<MyCoolWidget>();
```

Bu kod parçası eğer widget varsa döndürecek, yoksa hata verecektir:

```text
Element has no widget of type 'MySamples_MyCoolWidget'!
```

### WidgetExtensions.TryGetWidget Uzantı Metodu

Eğer element'in üzerinde widget var mı kontrol etmek isterseniz:

```cs
var myWidget = $('#SomeDiv').TryGetWidget<MyCoolWidget>();
```

`TryGetWidget`, eğer widget element'e bağlanmışsa bulup döndürür, yoksa hata vermek yerine `null` sonucunu verir.


### Aynı HTML elementinde Birden Çok Widget

Bir HTML elementine, aynı sınıftan tek bir widget bağlanabilir.

Aynı sınıftan ikinci bir widget oluşturmaya çalıştığınızda aşağıdaki gibi bir hata alırsınız:

```text
Element already has widget 'MySamples_MyCoolWidget'
```

Farklı sınıflardan, aynı elemente birden fazla Widget eklenebilir, tabi yaptıkları işlemlerin birbiriyle çakışmaması kaydıyla.

### *UniqueName* Özelliği

Oluşturulan her bir Widget, otomatik olarak eşsiz (unique) bir isim alır (`MySamples_MyCoolWidget3`) gibi.

`this.UniqueName` üzerinden erişilebilen bu ismi, Widget'ın bağlandığı eleman ya da içinde oluşturacağınız alt HTML elemanları için ID prefix'i (ön eki) olarak kullanarak, sayfa içindeki diğer Widget'larla çakışmayacak eşsiz ID'ler elde edebilirsiniz.

Ayrıca, event handler'larınızı jQuery üzerinden bu isim sınıfıyla bağlayıp (bind), daha sonra yine aynı sınıfla kaldırarak (unbind), aynı eleman üzerine atanmış diğer event handler'ları etkilemekten korunabilirsiniz:

```cs
jQuery("body").Bind("click." + this.UniqueName, delegate { ... });

...

jQUery("body").Unbind("click." + this.UniqueName);
```

### Widget.Destroy Metodu

Bir element üzerinde oluşturduğunuz widget'ı, bazen yoketmek (free) isteyebilirsiniz.

Bunun için Widget sınıfı `Destroy` metodunu sunar.

Destroy metodunun varsayılan implementasyonunda, element üzerindeki bu widget tarafından atanmış event handler'lar temizlenir (uniqueName class'ı kullanılarak) ve widget için eklenmiş CSS sınıfı (`.s-WidgetClass`) kaldırılır.

Özel Widget sınıflarında, Destroy metodu override edilerek, HTML elementi üzerinde yapılan değişiklikler geri alınabilir. 

Destroy metodu, HTML elementi DOM'dan ayrıldığında (detach) otomatik olarak çağrılır. Manuel de çağrılması mümkündür.

Destroy işlemi sağlıklı bir şekilde yapılmadığı taktirde, bazı browser'larda hafıza sızıntısı (memory leak) oluşması sözkonusu olabilir.

# Widget&lt;TOptions&gt; Generic Sınıfı

Eğer widget'iniz oluşturulurken, üzerinde oluşturulacağı HTML elementi dışında, bazı ek opsiyonlara ihtiyaç duyuyorsa, `Widget<TOptions>` sınıfını baz alabilirsiniz. 

Bu opsiyonlara, constructor'dan direk erişilebileceğiniz gibi, diğer sınıf metodları içinden, `this.options` özelliği aracılığıyla da erişilebilirsiniz.

```cs
public abstract class Widget<TOptions> : Widget
    where TOptions: class, new()
{
    protected Widget(jQueryObject element, TOptions opt = null) { ... }
    protected readonly TOptions options;
}
```

#TemplatedWidget Sınıfı

Geliştireceğiniz Widget, hedef aldığı HTML elementi içinde, aşağıdaki gibi, karmaşık bir HTML içerik oluşturacak ise, bu markup'ı direk Widget metodlarında üretmeniz çorba (spagetti) koda yol açabilir. Ayrıca markup kod ile üretildiğinden, ihtiyaca göre özelleştirilmesi de zorlaşır.

```cs
public class MyComplexWidget : Widget
{
	public MyComplexWidget(jQueryObject div)
    	: base(div)
    {
		var toolbar = J("<div>")
        	.Attribute("id", this.uniqueName + "_MyToolbar")
            .AppendTo(div);
        
        var table = J("<table>")
        	.AddClass("myTable")
            .Attribute("id", this.uniqueName + "_MyTable")
            .AppendTo(div);
            
        var header = J("<thead/>").AppendTo(table);
        var body = J("<tbody>").AppendTo(table);
        ...
        ...
        ...       
    }
}
```

Bu sorun HTML şablonu kullanarak çözülebilir. Örneğin aşağıdaki kodu HTML sayfa kodumuza ekleyelim:

```html
<script id="Template_MyComplexWidget" type="text/html">
<div id="~_MyToolbar">
</div>
<table id="~_MyTable">
	<thead><tr><th>Name</th><th>Surname</th>...</tr></thead>
    <tbody>...</tbody>
</table>
</script>
```

Burada `SCRIPT` tag'i kullandık, ancak tipini `"text/html"` yaparak, browser tarafından script olarak algılanmasını engelledik.

TemplatedWidget sınıfını baz alarak önceki kodu şu hale getirelim:

```cs
public class MyComplexWidget : TemplatedWidget
{
	public MyComplexWidget(jQueryObject div)
    	: base(div)
    {      
    }
}
```

Bu widget'ı aşağıdaki gibi bir HTML elementi üzerinde oluşturduğunuzda:

```html
<div id="SampleElement">
</div>
```

Şöyle bir görünüm elde edersiniz:

```html
<div id="SampleElement">
    <div id="MySamples_MyComplexWidget1_MyToolbar">
    </div>
    <table id="MySamples_MyComplexWidget1_MyTable">
        <thead><tr><th>Name</th><th>Surname</th>...</tr></thead>
        <tbody>...</tbody>
    </table>
</div>
```

TemplatedWidget, otomatik olarak sizin sınıfınız için hazırladığınız şablonu bulur ve HTML elementine uygular.

## TemplatedWidget ID Üretimi

Dikkat ederseniz şablonumuzda alt elementlerin ID'lerini `~_MyToolbar`, `~_MyTable` şeklinde yazdık. 

Ancak şablon HTML elementine uygulandığında üretilen ID'ler sırasıyla **MySamples_MyComplexWidget1_MyToolbar** ve `MySamples_MyComplexWidget1_MyTable` oldu.

TemplatedWidget, şablondaki `~_` öneklerini, Widget'ın eşsiz ismi (`uniqueName`) ve alt çizgi "_" ile değiştirir (`_` de içeren bu ön eke `this.idPrefix` alanı üzerinden erişilebilir).

Bu yolla, aynı Widget şablonu, aynı sayfada birden fazla HTML elementinde kullanılsa bile, ID'lerin çakışması önlenir.

## TemplatedWidget.ByID Metodu

ID'lerin başlarına TemplateWidget'ın eşsiz ismi getirildiğinden, şablon HTML elementine uygulandıktan sonra, üretilen alt HTML elemanlarına, şablonda belirttiğiniz ID'ler ile direk ulaşamazsınız.

elemanları ararken ID'lerinin başına TemplatedWidget'ın uniqueName'ini ve alt çizgi karakterini (`_`) getirmeniz gerekir:

```cs
public class MyComplexWidget : TemplatedWidget
{
	public MyComplexWidget(jQueryObject div)
    	: base(div)
    {
    	J(this.uniqueName + "_" + "Toolbar").AddClass("some-class");
    }
}
```

Bunun yerine TemplatedWidget'ın ByID metodu kullanılabilir:

```cs
public class MyComplexWidget
{
	public MyComplexWidget(jQueryObject div)
    	: base(div)
    {
    	ByID("Toolbar").AddClass("some-class");
    }
}
```

## TemplatedWidget.GetTemplateName Metodu

Örneğimizde, `MyComplexWidget` sınıfımız kullanacağı şablonu otomatik olarak bulmuştu. 

Bunun için TemplatedWidget bir çıkarım yaptı (convention based programming). Sınıfımızın class isminin (`MyComplexWidget`) başına `Template_` önekini getirdi ve bu ID'ye (`Template_MyComplexWidget`) sahip bir `SCRIPT` elementi arayıp, bulduğu elementin içeriğini şablon olarak kabul etti.

Eğer biz şablonumuzun ID'sini `Template_MyComplexWidget` yerine aşağıdaki gibi değiştirseydik:

```html
<script id="TheMyComplexWidgetTemplate" type="text/html">
	...
</script>
```

Bu durumda tarayıcı konsolunda şöyle bir hata alırdık:

```text
Can't locate template for widget 'MyComplexWidget' with name 'Template_MyComplexWidget'!
```

Şablonumuzun ID'sini düzeltebilir, ya da widget'imizin bu ismi kullanmasını sağlayabiliriz:

```cs
public class MyComplexWidget
{
	protected override string GetTemplateName()
    {
    	return "TheMyComplexWidgetTemplate";
    }
}
```

## TemplatedWidget.GetTemplate Metodu

Eğer şablonunuzu başka bir kaynaktan getirmek ya da manuel olarak oluşturmak isterseniz, `GetTemplate` metodunu override edebilirsiniz:

```cs
public class MyCompleWidget
{
	protected override string GetTemplate()
    {
    	return $('#TheMyComplexWidgetTemplate').GetHtml();
    }
}
```


## Q.GetTemplate Metodu ve Sunucu Tarafı Şablonları

TemplatedWidget.GetTemplate metodu varsayılan implementasyonunda, GetTemplateName metodunu çağırıp, bu ID'ye sahip `SCRIPT` elementinin HTML içeriğini şablon olarak döndürür.  

Eğer SCRIPT elementi bulunamaz ise, `Q.GetTemplate` metodu bu şablon ismiyle çağrılır. 

O da sonuç döndürmez ise şablonun tespit edilemediğine dair hata verilir.

`Q.GetTemplate` metodu ise, sunucu tarafında tanımlanmış şablonları getirir.  

Bu şablonlar, `~/Views/Template` ya da `~/Modules` klasörleri ve alt klasörlerinde bulunan `.template.cshtml` uzantılı dosyalardan derlenir.

Örneğin, MyComplexWidget için kullacağınız şablonu, sunucuda `~/Views/Template/SomeFolder/MyComplexWidget.template.cshtml` gibi bir dosyaya aşağıdaki içerikle yazabilirsiniz:

```html
<div id="~_MyToolbar">
</div>
<table id="~_MyTable">
	<thead><tr><th>Name</th><th>Surname</th>...</tr></thead>
    <tbody>...</tbody>
</table>
```

Dosyanın adı ve uzantısı önemlidir, hangi alt klasörde olduğunun bir önemi yoktur. 

Bu şekilde sayfanın HTML koduna herhangi bir widget şablonu eklenmesi gerekmez. 

Ayrıca bu şablonlar ilk ihtiyaç olduğunda yüklendiğinden (*lazy loading*), ve sunucu ve istemci tarafında önbelleklendiğinden (*cached*), sayfa kodunuz belki de hiç kullanmayacağınız widget'lar için gereksiz yere şişmez. Bu yüzden sayfa içi (inline) SCRIPT şablonları yerine, sunucu tarafı şablonları kullanmanız tavsiye edilir.


# TemplatedDialog Sınıfı

TemplatedWidget'ın alt sınıfı olan TemplatedDialog, jQuery UI Dialog plugin'i kullanarak sayfa içi modal dialog'lar oluşturmamızı sağlar.

Diğer widget türlerinden farklı olarak, TemplatedDialog, bağlanacağı HTML elementini kendi oluşturur.


```
.
.
.
.
.
.
.
.
.
.
.
```





  [1]: WidgetHierarchy.png