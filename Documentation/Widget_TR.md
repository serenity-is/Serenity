# ScriptContext Sınıfı
C#, Javascript'ten farklı olarak bir sınıfa bağlı olmayan global fonksiyonlara izin vermez. 

Bu nedenle jQuery'nin `$` fonksiyonunu Saltarelle ile aynı basitlikte kullanamayız. `$('#SomeElementId)` gibi basit bir javascript ifadesinin, Saltarelle'deki karşılığı `jQuery.Select("#SomeElementId")` dir.

Bunun için *ScriptContext* sınıfı geliştirilmiştir.

```cs
public class ScriptContext
{
    [InlineCode("$({p})")]
    protected static jQueryObject J(object p);
    [InlineCode("$({p}, {context})")]
    protected static jQueryObject J(object p, object context);
}
```

`$`, C#'da geçerli bir metod ismi (identifier) olmadığından onun yerine `J` kullanılmıştır. ScriptContext'ten türeyen sınıflarda, jQuery() fonksiyonuna kısaca `J()` ile erişilebilir.

```cs

public class SampleClass : ScriptContext
{
    public void SomeMethod()
    {
        J("#SomeElementId").AddClass("abc");
    }
}
```

# Widget Sınıfı

Serenity Script Arayüzü (*Serenity.UI*) katmanında, bileşen sınıfları (*component / control*) için, *jQuery UI*'daki *Widget Factory*'e benzeyen, ancak C#'a daha uygun tasarlanan bir yapı baz alınmıştır.

> Aşağıdaki adreslerde jQuery UI widget yapısı ile ilgili bilgi alınabilir:
>
> http://learn.jquery.com/jquery-ui/widget-factory/
>
> http://msdn.microsoft.com/en-us/library/hh404085.aspx

Widget, bir HTML elementi üzerinde oluşturulan, bu elemente ek özellikler (*behaviour*) kazandıran bir nesnedir. 

Örneğin IntegerEditor widget'ı, bir INPUT elementine uygulanır ve bu input üzerinde sayı girişini kolaylaştırıp, girilen verinin geçerli bir tamsayı olmasını doğrular (validasyon).

Benzer şekilde bir Toolbar widget'ı, bir DIV elementine uygulanır ve bu DIV'i butonları olan bir araç çubuğuna dönüştür (burada DIV sadece yer tutucu olarak görev görür).

## Widget Sınıf Diyagramı
![Widget Class Diagram][1]

## Örnek Bir Widget

Bir DIV'e her tıklandığında font büyüklüğünü arttıran basit bir widget yapalım:

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
<div id="SomeDiv">Örnek Metin</div>
```

Aşağıdaki gibi bu objeyi, HTML elementi üzerinde oluşturabiliriz:

```cs
var div = jQuery.Select("#SomeDiv");
new MyCoolWidget(div);
```

## Widget Sınıfı Metod ve Özellikleri
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

### *Widget.Element* Özelliği (Property)

Widget sınıfından türeyen nesneler, kendilerinin bağlı olduğu HTML elementine, `Element` özelliği üzerinden erişebilirler.

```cs
public jQueryObject Element { get; }
```

Bu özellik jQueryObject tipindedir ve widget oluşturulurken belirtilen elementi içerir. Örneğimizde, click olayı içerisinde elementimize (`div`), `this.Element` şeklinde eriştik. 

### HTML Elementi ve Widget *CSS Sınıfı* İlişkisi

Bir HTML elementi üzerinde bir widget oluşturduğunuzda, HTML elementinde bazı düzenlemeler yapılır.

İlk olarak HTML elementine, üzerinde oluşan widget'ın tip adına göre bir CSS sınıfı (class) eklenir.

Örneğimizdeki `#SomeDiv` ID'sine sahip `DIV` elementine `.s-MyCoolWidget` sınıfı eklenmiş oldu.

Yani DIV aşağıdaki gibi oldu:

```html
<div id="SomeDiv" class="s-MyCoolWidget">Örnek Metin</div>
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
var myWidget = J("#SomeDiv").TryGetWidget<MyCoolWidget>();
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
    	return J("#TheMyComplexWidgetTemplate").GetHtml();
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