/*! 
 * jquery.event.drag.live - v 2.2
 * Copyright (c) 2010 Three Dub Media - http://threedubmedia.com
 * Open Source MIT License - http://threedubmedia.com/code/license
 */
(function(n){var r="draginit.",i=n.event,t=i.special.drag,u=t.add,f=t.teardown;t.noBubble=!1,t.livekey="livedrag",t.add=function(f){u.apply(this,arguments);var e=n.data(this,t.datakey);!e.live&&f.selector&&(e.live=!0,i.add(this,r+t.livekey,t.delegate))},t.teardown=function(){f.apply(this,arguments);var u=n.data(this,t.datakey)||{};u.live&&(i.remove(this,r+t.livekey,t.delegate),u.live=!1)},t.delegate=function(r){var f=[],u,e=n.data(this,"events")||{};return(n.each(e||[],function(e,o){e.indexOf("drag")===0&&n.each(o||[],function(e,o){(u=n(r.target).closest(o.selector,r.currentTarget)[0],u)&&(i.add(u,o.origType+"."+t.livekey,o.origHandler||o.handler,o.data),n.inArray(u,f)<0&&f.push(u))})}),!f.length)?!1:n(f).bind("dragend."+t.livekey,function(){i.remove(this,"."+t.livekey)})}})(jQuery);