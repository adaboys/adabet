"use strict";(self.webpackChunk=self.webpackChunk||[]).push([[5789],{47664:(e,r,t)=>{t.r(r),t.d(r,{default:()=>c});var n=t(756),a=t(83606);function o(e,r){var t=Object.keys(e);if(Object.getOwnPropertySymbols){var n=Object.getOwnPropertySymbols(e);r&&(n=n.filter((function(r){return Object.getOwnPropertyDescriptor(e,r).enumerable}))),t.push.apply(t,n)}return t}function s(e){for(var r=1;r<arguments.length;r++){var t=null!=arguments[r]?arguments[r]:{};r%2?o(Object(t),!0).forEach((function(r){i(e,r,t[r])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(t)):o(Object(t)).forEach((function(r){Object.defineProperty(e,r,Object.getOwnPropertyDescriptor(t,r))}))}return e}function i(e,r,t){return r in e?Object.defineProperty(e,r,{value:t,enumerable:!0,configurable:!0,writable:!0}):e[r]=t,e}const u={middleware:["auth","verified","2fa_passed","admin"],data:function(){return{activeTab:null}},computed:s(s({},(0,t(20629).Se)({gameProvidersPackageIsEnabled:"package-manager/gameProvidersIsEnabled"})),{},{tabs:function(){var e=[{route:"admin.bonuses.general",name:this.$t("General")}];return this.gameProvidersPackageIsEnabled&&e.push({route:"admin.bonuses.providers.index",name:this.$t("Game providers")}),e}})};const c=(0,t(51900).Z)(u,(function(){var e=this,r=e._self._c;return r("div",[e.tabs.length>1?r(a.Z,{attrs:{centered:""}},e._l(e.tabs,(function(t,a){return r(n.Z,{key:a,attrs:{to:{name:t.route}}},[e._v("\n      "+e._s(t.name)+"\n    ")])})),1):e._e(),e._v(" "),r("router-view")],1)}),[],!1,null,null,null).exports}}]);