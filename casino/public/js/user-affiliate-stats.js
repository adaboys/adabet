"use strict";(self.webpackChunk=self.webpackChunk||[]).push([[8071],{90623:(t,e,r)=>{r.r(e),r.d(e,{default:()=>x});var n=r(85893),o=r(5255),i=r(17024),a=r(66530),s=r(57894),c=r(54437),u=r(27935),l=r(96878),f=r(73845),h=r(9669),p=r.n(h),v=r(20629),y=r(12169);function d(t){return d="function"==typeof Symbol&&"symbol"==typeof Symbol.iterator?function(t){return typeof t}:function(t){return t&&"function"==typeof Symbol&&t.constructor===Symbol&&t!==Symbol.prototype?"symbol":typeof t},d(t)}function m(){m=function(){return t};var t={},e=Object.prototype,r=e.hasOwnProperty,n=Object.defineProperty||function(t,e,r){t[e]=r.value},o="function"==typeof Symbol?Symbol:{},i=o.iterator||"@@iterator",a=o.asyncIterator||"@@asyncIterator",s=o.toStringTag||"@@toStringTag";function c(t,e,r){return Object.defineProperty(t,e,{value:r,enumerable:!0,configurable:!0,writable:!0}),t[e]}try{c({},"")}catch(t){c=function(t,e,r){return t[e]=r}}function u(t,e,r,o){var i=e&&e.prototype instanceof h?e:h,a=Object.create(i.prototype),s=new C(o||[]);return n(a,"_invoke",{value:O(t,r,s)}),a}function l(t,e,r){try{return{type:"normal",arg:t.call(e,r)}}catch(t){return{type:"throw",arg:t}}}t.wrap=u;var f={};function h(){}function p(){}function v(){}var y={};c(y,i,(function(){return this}));var _=Object.getPrototypeOf,g=_&&_(_(k([])));g&&g!==e&&r.call(g,i)&&(y=g);var b=v.prototype=h.prototype=Object.create(y);function w(t){["next","throw","return"].forEach((function(e){c(t,e,(function(t){return this._invoke(e,t)}))}))}function x(t,e){function o(n,i,a,s){var c=l(t[n],t,i);if("throw"!==c.type){var u=c.arg,f=u.value;return f&&"object"==d(f)&&r.call(f,"__await")?e.resolve(f.__await).then((function(t){o("next",t,a,s)}),(function(t){o("throw",t,a,s)})):e.resolve(f).then((function(t){u.value=t,a(u)}),(function(t){return o("throw",t,a,s)}))}s(c.arg)}var i;n(this,"_invoke",{value:function(t,r){function n(){return new e((function(e,n){o(t,r,e,n)}))}return i=i?i.then(n,n):n()}})}function O(t,e,r){var n="suspendedStart";return function(o,i){if("executing"===n)throw new Error("Generator is already running");if("completed"===n){if("throw"===o)throw i;return P()}for(r.method=o,r.arg=i;;){var a=r.delegate;if(a){var s=L(a,r);if(s){if(s===f)continue;return s}}if("next"===r.method)r.sent=r._sent=r.arg;else if("throw"===r.method){if("suspendedStart"===n)throw n="completed",r.arg;r.dispatchException(r.arg)}else"return"===r.method&&r.abrupt("return",r.arg);n="executing";var c=l(t,e,r);if("normal"===c.type){if(n=r.done?"completed":"suspendedYield",c.arg===f)continue;return{value:c.arg,done:r.done}}"throw"===c.type&&(n="completed",r.method="throw",r.arg=c.arg)}}}function L(t,e){var r=t.iterator[e.method];if(void 0===r){if(e.delegate=null,"throw"===e.method){if(t.iterator.return&&(e.method="return",e.arg=void 0,L(t,e),"throw"===e.method))return f;e.method="throw",e.arg=new TypeError("The iterator does not provide a 'throw' method")}return f}var n=l(r,t.iterator,e.arg);if("throw"===n.type)return e.method="throw",e.arg=n.arg,e.delegate=null,f;var o=n.arg;return o?o.done?(e[t.resultName]=o.value,e.next=t.nextLoc,"return"!==e.method&&(e.method="next",e.arg=void 0),e.delegate=null,f):o:(e.method="throw",e.arg=new TypeError("iterator result is not an object"),e.delegate=null,f)}function j(t){var e={tryLoc:t[0]};1 in t&&(e.catchLoc=t[1]),2 in t&&(e.finallyLoc=t[2],e.afterLoc=t[3]),this.tryEntries.push(e)}function E(t){var e=t.completion||{};e.type="normal",delete e.arg,t.completion=e}function C(t){this.tryEntries=[{tryLoc:"root"}],t.forEach(j,this),this.reset(!0)}function k(t){if(t){var e=t[i];if(e)return e.call(t);if("function"==typeof t.next)return t;if(!isNaN(t.length)){var n=-1,o=function e(){for(;++n<t.length;)if(r.call(t,n))return e.value=t[n],e.done=!1,e;return e.value=void 0,e.done=!0,e};return o.next=o}}return{next:P}}function P(){return{value:void 0,done:!0}}return p.prototype=v,n(b,"constructor",{value:v,configurable:!0}),n(v,"constructor",{value:p,configurable:!0}),p.displayName=c(v,s,"GeneratorFunction"),t.isGeneratorFunction=function(t){var e="function"==typeof t&&t.constructor;return!!e&&(e===p||"GeneratorFunction"===(e.displayName||e.name))},t.mark=function(t){return Object.setPrototypeOf?Object.setPrototypeOf(t,v):(t.__proto__=v,c(t,s,"GeneratorFunction")),t.prototype=Object.create(b),t},t.awrap=function(t){return{__await:t}},w(x.prototype),c(x.prototype,a,(function(){return this})),t.AsyncIterator=x,t.async=function(e,r,n,o,i){void 0===i&&(i=Promise);var a=new x(u(e,r,n,o),i);return t.isGeneratorFunction(r)?a:a.next().then((function(t){return t.done?t.value:a.next()}))},w(b),c(b,s,"Generator"),c(b,i,(function(){return this})),c(b,"toString",(function(){return"[object Generator]"})),t.keys=function(t){var e=Object(t),r=[];for(var n in e)r.push(n);return r.reverse(),function t(){for(;r.length;){var n=r.pop();if(n in e)return t.value=n,t.done=!1,t}return t.done=!0,t}},t.values=k,C.prototype={constructor:C,reset:function(t){if(this.prev=0,this.next=0,this.sent=this._sent=void 0,this.done=!1,this.delegate=null,this.method="next",this.arg=void 0,this.tryEntries.forEach(E),!t)for(var e in this)"t"===e.charAt(0)&&r.call(this,e)&&!isNaN(+e.slice(1))&&(this[e]=void 0)},stop:function(){this.done=!0;var t=this.tryEntries[0].completion;if("throw"===t.type)throw t.arg;return this.rval},dispatchException:function(t){if(this.done)throw t;var e=this;function n(r,n){return a.type="throw",a.arg=t,e.next=r,n&&(e.method="next",e.arg=void 0),!!n}for(var o=this.tryEntries.length-1;o>=0;--o){var i=this.tryEntries[o],a=i.completion;if("root"===i.tryLoc)return n("end");if(i.tryLoc<=this.prev){var s=r.call(i,"catchLoc"),c=r.call(i,"finallyLoc");if(s&&c){if(this.prev<i.catchLoc)return n(i.catchLoc,!0);if(this.prev<i.finallyLoc)return n(i.finallyLoc)}else if(s){if(this.prev<i.catchLoc)return n(i.catchLoc,!0)}else{if(!c)throw new Error("try statement without catch or finally");if(this.prev<i.finallyLoc)return n(i.finallyLoc)}}}},abrupt:function(t,e){for(var n=this.tryEntries.length-1;n>=0;--n){var o=this.tryEntries[n];if(o.tryLoc<=this.prev&&r.call(o,"finallyLoc")&&this.prev<o.finallyLoc){var i=o;break}}i&&("break"===t||"continue"===t)&&i.tryLoc<=e&&e<=i.finallyLoc&&(i=null);var a=i?i.completion:{};return a.type=t,a.arg=e,i?(this.method="next",this.next=i.finallyLoc,f):this.complete(a)},complete:function(t,e){if("throw"===t.type)throw t.arg;return"break"===t.type||"continue"===t.type?this.next=t.arg:"return"===t.type?(this.rval=this.arg=t.arg,this.method="return",this.next="end"):"normal"===t.type&&e&&(this.next=e),f},finish:function(t){for(var e=this.tryEntries.length-1;e>=0;--e){var r=this.tryEntries[e];if(r.finallyLoc===t)return this.complete(r.completion,r.afterLoc),E(r),f}},catch:function(t){for(var e=this.tryEntries.length-1;e>=0;--e){var r=this.tryEntries[e];if(r.tryLoc===t){var n=r.completion;if("throw"===n.type){var o=n.arg;E(r)}return o}}throw new Error("illegal catch attempt")},delegateYield:function(t,e,r){return this.delegate={iterator:k(t),resultName:e,nextLoc:r},"next"===this.method&&(this.arg=void 0),f}},t}function _(t,e,r,n,o,i,a){try{var s=t[i](a),c=s.value}catch(t){return void r(t)}s.done?e(c):Promise.resolve(c).then(n,o)}function g(t,e){var r=Object.keys(t);if(Object.getOwnPropertySymbols){var n=Object.getOwnPropertySymbols(t);e&&(n=n.filter((function(e){return Object.getOwnPropertyDescriptor(t,e).enumerable}))),r.push.apply(r,n)}return r}function b(t,e,r){return e in t?Object.defineProperty(t,e,{value:r,enumerable:!0,configurable:!0,writable:!0}):t[e]=r,t}const w={metaInfo:function(){return{title:this.$t("Affiliate program")}},data:function(){return{stats:null}},computed:function(t){for(var e=1;e<arguments.length;e++){var r=null!=arguments[e]?arguments[e]:{};e%2?g(Object(r),!0).forEach((function(e){b(t,e,r[e])})):Object.getOwnPropertyDescriptors?Object.defineProperties(t,Object.getOwnPropertyDescriptors(r)):g(Object(r)).forEach((function(e){Object.defineProperty(t,e,Object.getOwnPropertyDescriptor(r,e))}))}return t}({},(0,v.rn)("auth",["user"])),created:function(){var t,e=this;return(t=m().mark((function t(){var r,n;return m().wrap((function(t){for(;;)switch(t.prev=t.next){case 0:return t.next=2,p().get("/api/user/affiliate/stats");case 2:r=t.sent,n=r.data,e.stats=n;case 5:case"end":return t.stop()}}),t)})),function(){var e=this,r=arguments;return new Promise((function(n,o){var i=t.apply(e,r);function a(t){_(i,n,o,a,s,"next",t)}function s(t){_(i,n,o,a,s,"throw",t)}a(void 0)}))})()},methods:{decimal:y.decimal}};const x=(0,r(51900).Z)(w,(function(){var t=this,e=t._self._c;return e(a.Z,{attrs:{fluid:""}},[e(s.Z,{attrs:{justify:"center"}},[e(i.Z,{attrs:{cols:"12",lg:"6"}},[e(n.Z,[e(l.Z,[e(f.qW,[t._v("\n            "+t._s(t.$t("Stats"))+"\n          ")])],1),t._v(" "),e(o.ZB,[e("h2",{staticClass:"title"},[t._v(t._s(t.$t("Registrations")))]),t._v(" "),e(c.Z,{scopedSlots:t._u([{key:"default",fn:function(){return[e("thead",[e("tr",[e("th",{staticClass:"text-center"},[t._v(t._s(t.$t("Tier {0}",[1])))]),t._v(" "),e("th",{staticClass:"text-center"},[t._v(t._s(t.$t("Tier {0}",[2])))]),t._v(" "),e("th",{staticClass:"text-center"},[t._v(t._s(t.$t("Tier {0}",[3])))])])]),t._v(" "),e("tbody",[e("tr",[t.stats?[e("td",{staticClass:"text-center"},[t._v(t._s(t.stats.registrations.tier1_count))]),t._v(" "),e("td",{staticClass:"text-center"},[t._v(t._s(t.stats.registrations.tier2_count))]),t._v(" "),e("td",{staticClass:"text-center"},[t._v(t._s(t.stats.registrations.tier3_count))])]:[e("td",{attrs:{colspan:"3"}},[e(u.Z,{attrs:{type:"text"}})],1)]],2)])]},proxy:!0}])}),t._v(" "),e("h2",{staticClass:"title mt-5"},[t._v(t._s(t.$t("Commissions by tier")))]),t._v(" "),e(c.Z,{scopedSlots:t._u([{key:"default",fn:function(){return[e("thead",[e("tr",[e("th",{staticClass:"text-center"},[t._v(t._s(t.$t("Tier {0}",[1])))]),t._v(" "),e("th",{staticClass:"text-center"},[t._v(t._s(t.$t("Tier {0}",[2])))]),t._v(" "),e("th",{staticClass:"text-center"},[t._v(t._s(t.$t("Tier {0}",[3])))])])]),t._v(" "),e("tbody",[e("tr",[t.stats?t._l([1,2,3],(function(r){return e("td",{key:r,staticClass:"text-center"},[t._v("\n                      "+t._s(t.stats.commissions_by_tier[r]?t.decimal(t.stats.commissions_by_tier[r].commissions_total):"0.00")+"\n                    ")])})):[e("td",{attrs:{colspan:"3"}},[e(u.Z,{attrs:{type:"text"}})],1)]],2)])]},proxy:!0}])}),t._v(" "),e("h2",{staticClass:"title mt-5"},[t._v(t._s(t.$t("Commissions by type")))]),t._v(" "),e(c.Z,{scopedSlots:t._u([{key:"default",fn:function(){return[e("thead",[e("tr",[e("th",{staticClass:"text-left"},[t._v(t._s(t.$t("Type")))]),t._v(" "),e("th",{staticClass:"text-right"},[t._v(t._s(t.$t("Amount")))])])]),t._v(" "),e("tbody",[t.stats?[t.stats.commissions_by_type.length?t._l(t.stats.commissions_by_type,(function(r,n){return e("tr",{key:r.title},[e("td",{staticClass:"text-left"},[t._v(t._s(r.title))]),t._v(" "),e("td",{staticClass:"text-right"},[t._v(t._s(t.decimal(r.commissions_total)))])])})):[e("tr",[e("td",{attrs:{colspan:"2"}},[t._v("\n                        "+t._s(t.$t("No commissions found."))+"\n                      ")])])]]:t._l(Array(5).fill(0),(function(t,r){return e("tr",{key:r},[e("td",{attrs:{colspan:"2"}},[e(u.Z,{attrs:{type:"text"}})],1)])}))],2)]},proxy:!0}])})],1)],1)],1)],1)],1)}),[],!1,null,null,null).exports}}]);