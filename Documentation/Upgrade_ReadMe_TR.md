# Versiyon Geçişlerinde Dikkat Edilmesi Gerekenler

## ~\Views\Shared \ _LayoutHead.cshtml

Bu dosyadaki aşağıdakine benzer satırlar yeni jQuery ve jQuery UI versiyonlarıyla güncellenmeli:

```cs
    @Html.Script("~/Scripts/jquery-2.1.0.js")
    @Html.Script("~/Scripts/jquery-ui-1.10.4.js")
```

## ~\Content\slick*.css

Bu dosyaların ~\Content\slickgrid altına taşınması gerekli.

## ~\Content\images\*.gif

Bunlar SlickGrid'in dosyaları. ~\Content\slickgrid\images\ altına taşınmalılar.

~/Content/images klasörü daha sonra silinebilir.

## ~\Content\toastr*.css, toastr*.less

Bu dosyaların ~\Content\toastr altına taşınması gerekli.

## ~\Content\themes klasörü

Bu klasörde jQuery UI'ın teması var. Biz Aristo teması kullandığımızdan bu klasöre ihtiyacımız yok. Silinebilir.

## ~\Scripts\jquery.ui-x.xx.x.js ve jquery.ui-x.xx.x.min.js

Aşağıdakine benzer kod bulunup:


```js
	_moveToTop: function( event, silent ) {
		var moved = !!this.uiDialog.nextAll(":visible").insertBefore( this.uiDialog ).length;
		if ( moved && !silent ) {
			this._trigger( "focus", event );
		}

		return moved;
	}
```

Şu değişiklik yapılmalı:

```js
	    var moved = !!this.uiDialog.nextAll(".ui-front:visible").insertBefore(this.uiDialog).length;
```

.min.js'de de benzer değişiklik yapılmalı.

Aksi taktirde CKEditor'ün yazı türü seçimi gibi dropdown ları ve popup ları dialog içerisinde sorun çıkarıyor.



## ~\Scripts\jquery.validate.js ve jquery.validate.min.js

Aşağıdakine benzer kod bulunup:

```js
			function delegate(event) {
				var validator = $.data(this[0].form, "validator"),
					eventType = "on" + event.type.replace(/^validate/, ""),
					settings = validator.settings;
				if ( settings[eventType] && !this.is( settings.ignore ) ) {
					settings[eventType].call(validator, this[0], event);
				}
			}
```

Şu değişiklik yapılmalı:

```js
			function delegate(event) {
				if (!this[0].form) return; // bug fix
				var validator = $.data(this[0].form, "validator"),
					eventType = "on" + event.type.replace(/^validate/, ""),
					settings = validator.settings;
				if (!validator) return; // bug fix
				if ( settings[eventType] && !this.is( settings.ignore ) ) {
					settings[eventType].call(validator, this[0], event);
				}
			}
```

.min.js'de de benzer değişiklik yapılmalı.

Aksi taktirde jQuery validate free edilen objeler için garip script hataları oluşturabiliyor.

## ~\Scripts\SlickGrid\Plugins\slick.autotooltips.js

Aşağıdakine benzer kod bulunup:

```js
Aşağıdakine benzer kod bulunup:

```js
    function handleHeaderMouseEnter(e, args) {
      var column = args.column,
          $node = $(e.target).closest(".slick-header-column");
      if (!column.toolTip) {
        $node.attr("title", ($node.innerWidth() < $node[0].scrollWidth) ? column.name : "");
      }
    }

```


Şu değişiklik yapılmalı:

```js
    function handleHeaderMouseEnter(e, args) {
      var column = args.column,
          $node = $(e.target).closest(".slick-header-column");
      if (column && !column.toolTip) {
        $node.attr("title", ($node.innerWidth() < $node[0].scrollWidth) ? column.name : "");
      }
    }
```

Aksi taktirde kolon taşınırken SlickGrid de javascript hataları görülüyor.

