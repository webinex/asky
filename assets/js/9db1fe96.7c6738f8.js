"use strict";(self.webpackChunkdocs=self.webpackChunkdocs||[]).push([[141],{3905:(e,n,t)=>{t.d(n,{Zo:()=>p,kt:()=>f});var r=t(7294);function a(e,n,t){return n in e?Object.defineProperty(e,n,{value:t,enumerable:!0,configurable:!0,writable:!0}):e[n]=t,e}function i(e,n){var t=Object.keys(e);if(Object.getOwnPropertySymbols){var r=Object.getOwnPropertySymbols(e);n&&(r=r.filter((function(n){return Object.getOwnPropertyDescriptor(e,n).enumerable}))),t.push.apply(t,r)}return t}function l(e){for(var n=1;n<arguments.length;n++){var t=null!=arguments[n]?arguments[n]:{};n%2?i(Object(t),!0).forEach((function(n){a(e,n,t[n])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(t)):i(Object(t)).forEach((function(n){Object.defineProperty(e,n,Object.getOwnPropertyDescriptor(t,n))}))}return e}function o(e,n){if(null==e)return{};var t,r,a=function(e,n){if(null==e)return{};var t,r,a={},i=Object.keys(e);for(r=0;r<i.length;r++)t=i[r],n.indexOf(t)>=0||(a[t]=e[t]);return a}(e,n);if(Object.getOwnPropertySymbols){var i=Object.getOwnPropertySymbols(e);for(r=0;r<i.length;r++)t=i[r],n.indexOf(t)>=0||Object.prototype.propertyIsEnumerable.call(e,t)&&(a[t]=e[t])}return a}var s=r.createContext({}),c=function(e){var n=r.useContext(s),t=n;return e&&(t="function"==typeof e?e(n):l(l({},n),e)),t},p=function(e){var n=c(e.components);return r.createElement(s.Provider,{value:n},e.children)},d="mdxType",u={inlineCode:"code",wrapper:function(e){var n=e.children;return r.createElement(r.Fragment,{},n)}},m=r.forwardRef((function(e,n){var t=e.components,a=e.mdxType,i=e.originalType,s=e.parentName,p=o(e,["components","mdxType","originalType","parentName"]),d=c(t),m=a,f=d["".concat(s,".").concat(m)]||d[m]||u[m]||i;return t?r.createElement(f,l(l({ref:n},p),{},{components:t})):r.createElement(f,l({ref:n},p))}));function f(e,n){var t=arguments,a=n&&n.mdxType;if("string"==typeof e||a){var i=t.length,l=new Array(i);l[0]=m;var o={};for(var s in n)hasOwnProperty.call(n,s)&&(o[s]=n[s]);o.originalType=e,o[d]="string"==typeof e?e:a,l[1]=o;for(var c=2;c<i;c++)l[c]=t[c];return r.createElement.apply(null,l)}return r.createElement.apply(null,t)}m.displayName="MDXCreateElement"},9030:(e,n,t)=>{t.r(n),t.d(n,{assets:()=>s,contentTitle:()=>l,default:()=>u,frontMatter:()=>i,metadata:()=>o,toc:()=>c});var r=t(7462),a=(t(7294),t(3905));const i={sidebar_position:2},l="Child Field Map",o={unversionedId:"how-to/child-field-map",id:"how-to/child-field-map",title:"Child Field Map",description:"Asky support reusage of IAskyFieldMap definition",source:"@site/docs/how-to/child-field-map.md",sourceDirName:"how-to",slug:"/how-to/child-field-map",permalink:"/asky/docs/how-to/child-field-map",draft:!1,editUrl:"https://github.com/webinex/asky/tree/main/docs/docs/how-to/child-field-map.md",tags:[],version:"current",sidebarPosition:2,frontMatter:{sidebar_position:2},sidebar:"tutorialSidebar",previous:{title:"Collection Field Map",permalink:"/asky/docs/how-to/collection-field-map"}},s={},c=[],p={toc:c},d="wrapper";function u(e){let{components:n,...t}=e;return(0,a.kt)(d,(0,r.Z)({},p,t,{components:n,mdxType:"MDXLayout"}),(0,a.kt)("h1",{id:"child-field-map"},"Child Field Map"),(0,a.kt)("p",null,"Asky support reusage of ",(0,a.kt)("inlineCode",{parentName:"p"},"IAskyFieldMap<T>")," definition"),(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-csharp",metastring:'title="Human.cs"',title:'"Human.cs"'},"public class Human\n{\n    public Guid Id { get; set; }\n    public string Name { get; set; }\n}\n")),(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-csharp",metastring:'title="Car.cs"',title:'"Car.cs"'},"public class Car\n{\n    public Guid Id { get; set; }\n    public Human Owner { get; set; }\n    public string Model { get; set; }\n}\n")),(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-csharp",metastring:'title="HumanFieldMap.cs',title:'"HumanFieldMap.cs'},'public class HumanFieldMap : IAskyFieldMap<Human>\n{\n    public Expression<Func<Human, object>> this[string fieldId] => fieldId switch\n    {\n        "name" => x => x.Name,\n        _ => null,\n    };\n}\n')),(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-csharp",metastring:'title="CarFieldMap.cs"',title:'"CarFieldMap.cs"'},'public class CarFieldMap : IAskyFieldMap<Car>\n{\n    private readonly IAskyFieldMap<Human> _humanFieldMap;\n\n    // ...\n\n    public Expression<Func<Car, object>> this[string fieldId]\n    {\n        get\n        {\n            if (fieldId.StartsWith("owner."))\n            {\n                return AskyFieldMap.Forward<Car, Human>(x => x.Owner,\n                    _humanFieldMap, fieldId.Substring("owner.".Length));\n            }\n\n            return fieldId switch\n            {\n                "model" => x => x.Model,\n                _ => null,\n            };\n        }\n    }\n}\n')),(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-csharp",metastring:'title="CarRepository.cs"',title:'"CarRepository.cs"'},'public class CarRepository\n{\n    private readonly IAskyFieldMap<Car> _carFieldMap;\n\n    // ...\n\n    public async Task<Car[]> GetAllCamryOwnedByJohnsAsync()\n    {\n        var filterRule = FilterRule.And(\n            FilterRule.Eq("owner.name", "John"),\n            FilterRule.Eq("model", "Camry"));\n\n        // ...\n    }\n}\n')))}u.isMDXComponent=!0}}]);