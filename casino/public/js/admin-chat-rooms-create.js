"use strict";(self.webpackChunk=self.webpackChunk||[]).push([[4353],{8935:(t,r,e)=>{e.r(r),e.d(r,{default:()=>x});var n=e(15537),o=e(85893),i=e(5255),a=e(17024),c=e(66530),u=e(83240),s=e(91864),l=e(57894),f=e(22515),h=e(84002),d=e(96878),m=e(73845),p=e(50175),v=e.n(p),y=e(74944);function g(t){return g="function"==typeof Symbol&&"symbol"==typeof Symbol.iterator?function(t){return typeof t}:function(t){return t&&"function"==typeof Symbol&&t.constructor===Symbol&&t!==Symbol.prototype?"symbol":typeof t},g(t)}function w(){w=function(){return t};var t={},r=Object.prototype,e=r.hasOwnProperty,n=Object.defineProperty||function(t,r,e){t[r]=e.value},o="function"==typeof Symbol?Symbol:{},i=o.iterator||"@@iterator",a=o.asyncIterator||"@@asyncIterator",c=o.toStringTag||"@@toStringTag";function u(t,r,e){return Object.defineProperty(t,r,{value:e,enumerable:!0,configurable:!0,writable:!0}),t[r]}try{u({},"")}catch(t){u=function(t,r,e){return t[r]=e}}function s(t,r,e,o){var i=r&&r.prototype instanceof h?r:h,a=Object.create(i.prototype),c=new j(o||[]);return n(a,"_invoke",{value:L(t,e,c)}),a}function l(t,r,e){try{return{type:"normal",arg:t.call(r,e)}}catch(t){return{type:"throw",arg:t}}}t.wrap=s;var f={};function h(){}function d(){}function m(){}var p={};u(p,i,(function(){return this}));var v=Object.getPrototypeOf,y=v&&v(v(O([])));y&&y!==r&&e.call(y,i)&&(p=y);var b=m.prototype=h.prototype=Object.create(p);function _(t){["next","throw","return"].forEach((function(r){u(t,r,(function(t){return this._invoke(r,t)}))}))}function x(t,r){function o(n,i,a,c){var u=l(t[n],t,i);if("throw"!==u.type){var s=u.arg,f=s.value;return f&&"object"==g(f)&&e.call(f,"__await")?r.resolve(f.__await).then((function(t){o("next",t,a,c)}),(function(t){o("throw",t,a,c)})):r.resolve(f).then((function(t){s.value=t,a(s)}),(function(t){return o("throw",t,a,c)}))}c(u.arg)}var i;n(this,"_invoke",{value:function(t,e){function n(){return new r((function(r,n){o(t,e,r,n)}))}return i=i?i.then(n,n):n()}})}function L(t,r,e){var n="suspendedStart";return function(o,i){if("executing"===n)throw new Error("Generator is already running");if("completed"===n){if("throw"===o)throw i;return S()}for(e.method=o,e.arg=i;;){var a=e.delegate;if(a){var c=Z(a,e);if(c){if(c===f)continue;return c}}if("next"===e.method)e.sent=e._sent=e.arg;else if("throw"===e.method){if("suspendedStart"===n)throw n="completed",e.arg;e.dispatchException(e.arg)}else"return"===e.method&&e.abrupt("return",e.arg);n="executing";var u=l(t,r,e);if("normal"===u.type){if(n=e.done?"completed":"suspendedYield",u.arg===f)continue;return{value:u.arg,done:e.done}}"throw"===u.type&&(n="completed",e.method="throw",e.arg=u.arg)}}}function Z(t,r){var e=t.iterator[r.method];if(void 0===e){if(r.delegate=null,"throw"===r.method){if(t.iterator.return&&(r.method="return",r.arg=void 0,Z(t,r),"throw"===r.method))return f;r.method="throw",r.arg=new TypeError("The iterator does not provide a 'throw' method")}return f}var n=l(e,t.iterator,r.arg);if("throw"===n.type)return r.method="throw",r.arg=n.arg,r.delegate=null,f;var o=n.arg;return o?o.done?(r[t.resultName]=o.value,r.next=t.nextLoc,"return"!==r.method&&(r.method="next",r.arg=void 0),r.delegate=null,f):o:(r.method="throw",r.arg=new TypeError("iterator result is not an object"),r.delegate=null,f)}function E(t){var r={tryLoc:t[0]};1 in t&&(r.catchLoc=t[1]),2 in t&&(r.finallyLoc=t[2],r.afterLoc=t[3]),this.tryEntries.push(r)}function k(t){var r=t.completion||{};r.type="normal",delete r.arg,t.completion=r}function j(t){this.tryEntries=[{tryLoc:"root"}],t.forEach(E,this),this.reset(!0)}function O(t){if(t){var r=t[i];if(r)return r.call(t);if("function"==typeof t.next)return t;if(!isNaN(t.length)){var n=-1,o=function r(){for(;++n<t.length;)if(e.call(t,n))return r.value=t[n],r.done=!1,r;return r.value=void 0,r.done=!0,r};return o.next=o}}return{next:S}}function S(){return{value:void 0,done:!0}}return d.prototype=m,n(b,"constructor",{value:m,configurable:!0}),n(m,"constructor",{value:d,configurable:!0}),d.displayName=u(m,c,"GeneratorFunction"),t.isGeneratorFunction=function(t){var r="function"==typeof t&&t.constructor;return!!r&&(r===d||"GeneratorFunction"===(r.displayName||r.name))},t.mark=function(t){return Object.setPrototypeOf?Object.setPrototypeOf(t,m):(t.__proto__=m,u(t,c,"GeneratorFunction")),t.prototype=Object.create(b),t},t.awrap=function(t){return{__await:t}},_(x.prototype),u(x.prototype,a,(function(){return this})),t.AsyncIterator=x,t.async=function(r,e,n,o,i){void 0===i&&(i=Promise);var a=new x(s(r,e,n,o),i);return t.isGeneratorFunction(e)?a:a.next().then((function(t){return t.done?t.value:a.next()}))},_(b),u(b,c,"Generator"),u(b,i,(function(){return this})),u(b,"toString",(function(){return"[object Generator]"})),t.keys=function(t){var r=Object(t),e=[];for(var n in r)e.push(n);return e.reverse(),function t(){for(;e.length;){var n=e.pop();if(n in r)return t.value=n,t.done=!1,t}return t.done=!0,t}},t.values=O,j.prototype={constructor:j,reset:function(t){if(this.prev=0,this.next=0,this.sent=this._sent=void 0,this.done=!1,this.delegate=null,this.method="next",this.arg=void 0,this.tryEntries.forEach(k),!t)for(var r in this)"t"===r.charAt(0)&&e.call(this,r)&&!isNaN(+r.slice(1))&&(this[r]=void 0)},stop:function(){this.done=!0;var t=this.tryEntries[0].completion;if("throw"===t.type)throw t.arg;return this.rval},dispatchException:function(t){if(this.done)throw t;var r=this;function n(e,n){return a.type="throw",a.arg=t,r.next=e,n&&(r.method="next",r.arg=void 0),!!n}for(var o=this.tryEntries.length-1;o>=0;--o){var i=this.tryEntries[o],a=i.completion;if("root"===i.tryLoc)return n("end");if(i.tryLoc<=this.prev){var c=e.call(i,"catchLoc"),u=e.call(i,"finallyLoc");if(c&&u){if(this.prev<i.catchLoc)return n(i.catchLoc,!0);if(this.prev<i.finallyLoc)return n(i.finallyLoc)}else if(c){if(this.prev<i.catchLoc)return n(i.catchLoc,!0)}else{if(!u)throw new Error("try statement without catch or finally");if(this.prev<i.finallyLoc)return n(i.finallyLoc)}}}},abrupt:function(t,r){for(var n=this.tryEntries.length-1;n>=0;--n){var o=this.tryEntries[n];if(o.tryLoc<=this.prev&&e.call(o,"finallyLoc")&&this.prev<o.finallyLoc){var i=o;break}}i&&("break"===t||"continue"===t)&&i.tryLoc<=r&&r<=i.finallyLoc&&(i=null);var a=i?i.completion:{};return a.type=t,a.arg=r,i?(this.method="next",this.next=i.finallyLoc,f):this.complete(a)},complete:function(t,r){if("throw"===t.type)throw t.arg;return"break"===t.type||"continue"===t.type?this.next=t.arg:"return"===t.type?(this.rval=this.arg=t.arg,this.method="return",this.next="end"):"normal"===t.type&&r&&(this.next=r),f},finish:function(t){for(var r=this.tryEntries.length-1;r>=0;--r){var e=this.tryEntries[r];if(e.finallyLoc===t)return this.complete(e.completion,e.afterLoc),k(e),f}},catch:function(t){for(var r=this.tryEntries.length-1;r>=0;--r){var e=this.tryEntries[r];if(e.tryLoc===t){var n=e.completion;if("throw"===n.type){var o=n.arg;k(e)}return o}}throw new Error("illegal catch attempt")},delegateYield:function(t,r,e){return this.delegate={iterator:O(t),resultName:r,nextLoc:e},"next"===this.method&&(this.arg=void 0),f}},t}function b(t,r,e,n,o,i,a){try{var c=t[i](a),u=c.value}catch(t){return void e(t)}c.done?r(u):Promise.resolve(u).then(n,o)}const _={components:{ChatRoomMenu:e(11181).Z},middleware:["auth","verified","2fa_passed","admin"],mixins:[y.Z],props:["id"],metaInfo:function(){return{title:this.$t("Create chat room",[this.id])}},data:function(){return{form:new(v())({name:null})}},methods:{create:function(){var t,r=this;return(t=w().mark((function t(){return w().wrap((function(t){for(;;)switch(t.prev=t.next){case 0:return t.next=2,r.form.post("/api/admin/chat/rooms");case 2:r.$store.dispatch("message/success",{text:r.$t("Chat room successfully created.")}),r.$router.push({name:"admin.chat.rooms.index"});case 4:case"end":return t.stop()}}),t)})),function(){var r=this,e=arguments;return new Promise((function(n,o){var i=t.apply(r,e);function a(t){b(i,n,o,a,c,"next",t)}function c(t){b(i,n,o,a,c,"throw",t)}a(void 0)}))})()}}};const x=(0,e(51900).Z)(_,(function(){var t=this,r=t._self._c;return r(c.Z,[r(l.Z,{attrs:{align:"center",justify:"center"}},[r(a.Z,{attrs:{cols:"12",md:"6"}},[r(o.Z,[r(d.Z,[r(n.Z,{attrs:{icon:""},on:{click:function(r){return t.$router.go(-1)}}},[r(s.Z,[t._v("mdi-arrow-left")])],1),t._v(" "),r(m.qW,[t._v("\n            "+t._s(t.$t("Create chat room"))+"\n          ")]),t._v(" "),r(f.Z),t._v(" "),r("chat-room-menu",{attrs:{id:t.id}})],1),t._v(" "),r(i.ZB,[r(u.Z,{on:{submit:function(r){return r.preventDefault(),t.create.apply(null,arguments)}},model:{value:t.formIsValid,callback:function(r){t.formIsValid=r},expression:"formIsValid"}},[r(h.Z,{attrs:{label:t.$t("Name"),disabled:t.form.busy,rules:[t.validationRequired],error:t.form.errors.has("name"),"error-messages":t.form.errors.get("name"),outlined:""},on:{keydown:function(r){return t.clearFormErrors(r,"name")}},model:{value:t.form.name,callback:function(r){t.$set(t.form,"name",r)},expression:"form.name"}}),t._v(" "),r(n.Z,{attrs:{type:"submit",color:"primary",disabled:!t.formIsValid||t.form.busy,loading:t.form.busy}},[t._v("\n              "+t._s(t.$t("Save"))+"\n            ")])],1)],1)],1)],1)],1)],1)}),[],!1,null,null,null).exports},62167:(t,r,e)=>{e.d(r,{Z:()=>n});const n={props:["id","small"]}},41760:(t,r,e)=>{e.d(r,{s:()=>l,x:()=>f});var n=e(15537),o=e(91864),i=e(48694),a=e(15151),c=e(53146),u=e(33560),s=e(27099),l=function(){var t=this,r=t._self._c;return r(s.Z,{attrs:{"offset-y":!0,transition:"scroll-y-transition",bottom:"",left:""},scopedSlots:t._u([{key:"activator",fn:function(e){var i=e.on;return[r(n.Z,t._g({attrs:{icon:""}},i),[r(o.Z,{attrs:{small:t.small}},[t._v("mdi-dots-vertical")])],1)]}}])},[t._v(" "),r(i.Z,[r(a.Z,{attrs:{to:{name:"admin.chat.rooms.edit",params:{id:t.id}},exact:""}},[r(u.Z,[r(o.Z,{attrs:{small:t.small}},[t._v("mdi-pencil")])],1),t._v(" "),r(c.km,[r(c.V9,[t._v(t._s(t.$t("Edit")))])],1)],1),t._v(" "),r(a.Z,{attrs:{to:{name:"admin.chat.rooms.delete",params:{id:t.id}},exact:""}},[r(u.Z,[r(o.Z,{attrs:{small:t.small}},[t._v("mdi-delete")])],1),t._v(" "),r(c.km,[r(c.V9,[t._v(t._s(t.$t("Delete")))])],1)],1)],1)],1)},f=[]}}]);