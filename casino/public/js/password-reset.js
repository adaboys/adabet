"use strict";(self.webpackChunk=self.webpackChunk||[]).push([[1226],{59205:(t,r,e)=>{e.r(r),e.d(r,{default:()=>L});var o=e(99856),n=e(15537),i=e(85893),a=e(5255),s=e(17024),c=e(66530),u=e(83240),l=e(21933),f=e(57894),h=e(22515),p=e(84002),d=e(96878),m=e(73845),v=e(80010),y=e(50175),w=e.n(y);function g(t){return g="function"==typeof Symbol&&"symbol"==typeof Symbol.iterator?function(t){return typeof t}:function(t){return t&&"function"==typeof Symbol&&t.constructor===Symbol&&t!==Symbol.prototype?"symbol":typeof t},g(t)}function b(){b=function(){return t};var t={},r=Object.prototype,e=r.hasOwnProperty,o=Object.defineProperty||function(t,r,e){t[r]=e.value},n="function"==typeof Symbol?Symbol:{},i=n.iterator||"@@iterator",a=n.asyncIterator||"@@asyncIterator",s=n.toStringTag||"@@toStringTag";function c(t,r,e){return Object.defineProperty(t,r,{value:e,enumerable:!0,configurable:!0,writable:!0}),t[r]}try{c({},"")}catch(t){c=function(t,r,e){return t[r]=e}}function u(t,r,e,n){var i=r&&r.prototype instanceof h?r:h,a=Object.create(i.prototype),s=new Z(n||[]);return o(a,"_invoke",{value:L(t,e,s)}),a}function l(t,r,e){try{return{type:"normal",arg:t.call(r,e)}}catch(t){return{type:"throw",arg:t}}}t.wrap=u;var f={};function h(){}function p(){}function d(){}var m={};c(m,i,(function(){return this}));var v=Object.getPrototypeOf,y=v&&v(v(j([])));y&&y!==r&&e.call(y,i)&&(m=y);var w=d.prototype=h.prototype=Object.create(m);function x(t){["next","throw","return"].forEach((function(r){c(t,r,(function(t){return this._invoke(r,t)}))}))}function _(t,r){function n(o,i,a,s){var c=l(t[o],t,i);if("throw"!==c.type){var u=c.arg,f=u.value;return f&&"object"==g(f)&&e.call(f,"__await")?r.resolve(f.__await).then((function(t){n("next",t,a,s)}),(function(t){n("throw",t,a,s)})):r.resolve(f).then((function(t){u.value=t,a(u)}),(function(t){return n("throw",t,a,s)}))}s(c.arg)}var i;o(this,"_invoke",{value:function(t,e){function o(){return new r((function(r,o){n(t,e,r,o)}))}return i=i?i.then(o,o):o()}})}function L(t,r,e){var o="suspendedStart";return function(n,i){if("executing"===o)throw new Error("Generator is already running");if("completed"===o){if("throw"===n)throw i;return O()}for(e.method=n,e.arg=i;;){var a=e.delegate;if(a){var s=k(a,e);if(s){if(s===f)continue;return s}}if("next"===e.method)e.sent=e._sent=e.arg;else if("throw"===e.method){if("suspendedStart"===o)throw o="completed",e.arg;e.dispatchException(e.arg)}else"return"===e.method&&e.abrupt("return",e.arg);o="executing";var c=l(t,r,e);if("normal"===c.type){if(o=e.done?"completed":"suspendedYield",c.arg===f)continue;return{value:c.arg,done:e.done}}"throw"===c.type&&(o="completed",e.method="throw",e.arg=c.arg)}}}function k(t,r){var e=t.iterator[r.method];if(void 0===e){if(r.delegate=null,"throw"===r.method){if(t.iterator.return&&(r.method="return",r.arg=void 0,k(t,r),"throw"===r.method))return f;r.method="throw",r.arg=new TypeError("The iterator does not provide a 'throw' method")}return f}var o=l(e,t.iterator,r.arg);if("throw"===o.type)return r.method="throw",r.arg=o.arg,r.delegate=null,f;var n=o.arg;return n?n.done?(r[t.resultName]=n.value,r.next=t.nextLoc,"return"!==r.method&&(r.method="next",r.arg=void 0),r.delegate=null,f):n:(r.method="throw",r.arg=new TypeError("iterator result is not an object"),r.delegate=null,f)}function E(t){var r={tryLoc:t[0]};1 in t&&(r.catchLoc=t[1]),2 in t&&(r.finallyLoc=t[2],r.afterLoc=t[3]),this.tryEntries.push(r)}function P(t){var r=t.completion||{};r.type="normal",delete r.arg,t.completion=r}function Z(t){this.tryEntries=[{tryLoc:"root"}],t.forEach(E,this),this.reset(!0)}function j(t){if(t){var r=t[i];if(r)return r.call(t);if("function"==typeof t.next)return t;if(!isNaN(t.length)){var o=-1,n=function r(){for(;++o<t.length;)if(e.call(t,o))return r.value=t[o],r.done=!1,r;return r.value=void 0,r.done=!0,r};return n.next=n}}return{next:O}}function O(){return{value:void 0,done:!0}}return p.prototype=d,o(w,"constructor",{value:d,configurable:!0}),o(d,"constructor",{value:p,configurable:!0}),p.displayName=c(d,s,"GeneratorFunction"),t.isGeneratorFunction=function(t){var r="function"==typeof t&&t.constructor;return!!r&&(r===p||"GeneratorFunction"===(r.displayName||r.name))},t.mark=function(t){return Object.setPrototypeOf?Object.setPrototypeOf(t,d):(t.__proto__=d,c(t,s,"GeneratorFunction")),t.prototype=Object.create(w),t},t.awrap=function(t){return{__await:t}},x(_.prototype),c(_.prototype,a,(function(){return this})),t.AsyncIterator=_,t.async=function(r,e,o,n,i){void 0===i&&(i=Promise);var a=new _(u(r,e,o,n),i);return t.isGeneratorFunction(e)?a:a.next().then((function(t){return t.done?t.value:a.next()}))},x(w),c(w,s,"Generator"),c(w,i,(function(){return this})),c(w,"toString",(function(){return"[object Generator]"})),t.keys=function(t){var r=Object(t),e=[];for(var o in r)e.push(o);return e.reverse(),function t(){for(;e.length;){var o=e.pop();if(o in r)return t.value=o,t.done=!1,t}return t.done=!0,t}},t.values=j,Z.prototype={constructor:Z,reset:function(t){if(this.prev=0,this.next=0,this.sent=this._sent=void 0,this.done=!1,this.delegate=null,this.method="next",this.arg=void 0,this.tryEntries.forEach(P),!t)for(var r in this)"t"===r.charAt(0)&&e.call(this,r)&&!isNaN(+r.slice(1))&&(this[r]=void 0)},stop:function(){this.done=!0;var t=this.tryEntries[0].completion;if("throw"===t.type)throw t.arg;return this.rval},dispatchException:function(t){if(this.done)throw t;var r=this;function o(e,o){return a.type="throw",a.arg=t,r.next=e,o&&(r.method="next",r.arg=void 0),!!o}for(var n=this.tryEntries.length-1;n>=0;--n){var i=this.tryEntries[n],a=i.completion;if("root"===i.tryLoc)return o("end");if(i.tryLoc<=this.prev){var s=e.call(i,"catchLoc"),c=e.call(i,"finallyLoc");if(s&&c){if(this.prev<i.catchLoc)return o(i.catchLoc,!0);if(this.prev<i.finallyLoc)return o(i.finallyLoc)}else if(s){if(this.prev<i.catchLoc)return o(i.catchLoc,!0)}else{if(!c)throw new Error("try statement without catch or finally");if(this.prev<i.finallyLoc)return o(i.finallyLoc)}}}},abrupt:function(t,r){for(var o=this.tryEntries.length-1;o>=0;--o){var n=this.tryEntries[o];if(n.tryLoc<=this.prev&&e.call(n,"finallyLoc")&&this.prev<n.finallyLoc){var i=n;break}}i&&("break"===t||"continue"===t)&&i.tryLoc<=r&&r<=i.finallyLoc&&(i=null);var a=i?i.completion:{};return a.type=t,a.arg=r,i?(this.method="next",this.next=i.finallyLoc,f):this.complete(a)},complete:function(t,r){if("throw"===t.type)throw t.arg;return"break"===t.type||"continue"===t.type?this.next=t.arg:"return"===t.type?(this.rval=this.arg=t.arg,this.method="return",this.next="end"):"normal"===t.type&&r&&(this.next=r),f},finish:function(t){for(var r=this.tryEntries.length-1;r>=0;--r){var e=this.tryEntries[r];if(e.finallyLoc===t)return this.complete(e.completion,e.afterLoc),P(e),f}},catch:function(t){for(var r=this.tryEntries.length-1;r>=0;--r){var e=this.tryEntries[r];if(e.tryLoc===t){var o=e.completion;if("throw"===o.type){var n=o.arg;P(e)}return n}}throw new Error("illegal catch attempt")},delegateYield:function(t,r,e){return this.delegate={iterator:j(t),resultName:r,nextLoc:e},"next"===this.method&&(this.arg=void 0),f}},t}function x(t,r,e,o,n,i,a){try{var s=t[i](a),c=s.value}catch(t){return void e(t)}s.done?r(c):Promise.resolve(c).then(o,n)}const _={mixins:[e(74944).Z],middleware:"guest",metaInfo:function(){return{title:this.$t("Password reset")}},data:function(){return{showPassword:!1,showPassword2:!1,form:new(w())({token:null,email:null,password:null,password_confirmation:null})}},computed:{appLogoUrl:function(){return(0,v.v)("app.logo")}},created:function(){this.form.email=this.$route.query.email,this.form.token=this.$route.params.token},methods:{reset:function(){var t,r=this;return(t=b().mark((function t(){var e,o;return b().wrap((function(t){for(;;)switch(t.prev=t.next){case 0:return t.next=2,r.form.post("/api/auth/password/reset");case 2:return e=t.sent,o=e.data,t.next=6,r.$store.dispatch("auth/fetchUser");case 6:r.$store.dispatch("message/success",{text:o.message}),r.$router.push({name:"home"});case 8:case"end":return t.stop()}}),t)})),function(){var r=this,e=arguments;return new Promise((function(o,n){var i=t.apply(r,e);function a(t){x(i,o,n,a,s,"next",t)}function s(t){x(i,o,n,a,s,"throw",t)}a(void 0)}))})()}}};const L=(0,e(51900).Z)(_,(function(){var t=this,r=t._self._c;return r(c.Z,{staticClass:"fill-height",attrs:{fluid:""}},[r(f.Z,{attrs:{align:"center",justify:"center"}},[r(s.Z,{attrs:{cols:"12",sm:"8",md:"6",lg:"4"}},[r(i.Z,{staticClass:"elevation-12"},[r(d.Z,{attrs:{color:"primary"}},[r("router-link",{attrs:{to:{name:"home"}}},[r(o.Z,{attrs:{size:"40",tile:""}},[r(l.Z,{attrs:{src:t.appLogoUrl}})],1)],1),t._v(" "),r(m.qW,{staticClass:"ml-2"},[t._v("\n            "+t._s(t.$t("Password reset"))+"\n          ")]),t._v(" "),r(h.Z)],1),t._v(" "),r(a.ZB,[r(u.Z,{ref:"form",on:{submit:function(r){return r.preventDefault(),t.reset.apply(null,arguments)}},model:{value:t.formIsValid,callback:function(r){t.formIsValid=r},expression:"formIsValid"}},[r(p.Z,{attrs:{label:t.$t("Email"),type:"email",name:"email",error:t.form.errors.has("email"),"error-messages":t.form.errors.get("email"),outlined:"",readonly:""},model:{value:t.form.email,callback:function(r){t.$set(t.form,"email",r)},expression:"form.email"}}),t._v(" "),r(p.Z,{attrs:{label:t.$t("Password"),"append-icon":t.showPassword?"mdi-eye":"mdi-eye-off",type:t.showPassword?"text":"password",name:"password",rules:[t.validationRequired,function(r){return t.validationMinLength(r,6)}],error:t.form.errors.has("password"),"error-messages":t.form.errors.get("password"),outlined:"",counter:""},on:{"click:append":function(r){t.showPassword=!t.showPassword},keydown:function(r){return t.clearFormErrors(r,"password")}},model:{value:t.form.password,callback:function(r){t.$set(t.form,"password",r)},expression:"form.password"}}),t._v(" "),r(p.Z,{attrs:{label:t.$t("Confirm password"),"append-icon":t.showPassword2?"mdi-eye":"mdi-eye-off",type:t.showPassword2?"text":"password",name:"password_confirmation",rules:[t.validationRequired,function(r){return t.validationMatch(t.form.password,r)}],error:t.form.errors.has("password_confirmation"),"error-messages":t.form.errors.get("password_confirmation"),outlined:"",counter:""},on:{"click:append":function(r){t.showPassword2=!t.showPassword2},keydown:function(r){return t.clearFormErrors(r,"password_confirmation")}},model:{value:t.form.password_confirmation,callback:function(r){t.$set(t.form,"password_confirmation",r)},expression:"form.password_confirmation"}}),t._v(" "),r(n.Z,{attrs:{type:"submit",color:"primary",disabled:!t.formIsValid||t.form.busy,loading:t.form.busy}},[t._v("\n              "+t._s(t.$t("Reset"))+"\n            ")])],1)],1)],1)],1)],1)],1)}),[],!1,null,null,null).exports}}]);