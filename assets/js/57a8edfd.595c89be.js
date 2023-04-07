"use strict";(self.webpackChunkdocs=self.webpackChunkdocs||[]).push([[825],{3905:(e,t,n)=>{n.d(t,{Zo:()=>p,kt:()=>f});var r=n(7294);function i(e,t,n){return t in e?Object.defineProperty(e,t,{value:n,enumerable:!0,configurable:!0,writable:!0}):e[t]=n,e}function l(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var r=Object.getOwnPropertySymbols(e);t&&(r=r.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),n.push.apply(n,r)}return n}function a(e){for(var t=1;t<arguments.length;t++){var n=null!=arguments[t]?arguments[t]:{};t%2?l(Object(n),!0).forEach((function(t){i(e,t,n[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):l(Object(n)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(n,t))}))}return e}function o(e,t){if(null==e)return{};var n,r,i=function(e,t){if(null==e)return{};var n,r,i={},l=Object.keys(e);for(r=0;r<l.length;r++)n=l[r],t.indexOf(n)>=0||(i[n]=e[n]);return i}(e,t);if(Object.getOwnPropertySymbols){var l=Object.getOwnPropertySymbols(e);for(r=0;r<l.length;r++)n=l[r],t.indexOf(n)>=0||Object.prototype.propertyIsEnumerable.call(e,n)&&(i[n]=e[n])}return i}var c=r.createContext({}),s=function(e){var t=r.useContext(c),n=t;return e&&(n="function"==typeof e?e(t):a(a({},t),e)),n},p=function(e){var t=s(e.components);return r.createElement(c.Provider,{value:t},e.children)},d="mdxType",u={inlineCode:"code",wrapper:function(e){var t=e.children;return r.createElement(r.Fragment,{},t)}},m=r.forwardRef((function(e,t){var n=e.components,i=e.mdxType,l=e.originalType,c=e.parentName,p=o(e,["components","mdxType","originalType","parentName"]),d=s(n),m=i,f=d["".concat(c,".").concat(m)]||d[m]||u[m]||l;return n?r.createElement(f,a(a({ref:t},p),{},{components:n})):r.createElement(f,a({ref:t},p))}));function f(e,t){var n=arguments,i=t&&t.mdxType;if("string"==typeof e||i){var l=n.length,a=new Array(l);a[0]=m;var o={};for(var c in t)hasOwnProperty.call(t,c)&&(o[c]=t[c]);o.originalType=e,o[d]="string"==typeof e?e:i,a[1]=o;for(var s=2;s<l;s++)a[s]=n[s];return r.createElement.apply(null,a)}return r.createElement.apply(null,n)}m.displayName="MDXCreateElement"},9633:(e,t,n)=>{n.r(t),n.d(t,{assets:()=>c,contentTitle:()=>a,default:()=>u,frontMatter:()=>l,metadata:()=>o,toc:()=>s});var r=n(7462),i=(n(7294),n(3905));const l={sidebar_position:1},a="Collection Field Map",o={unversionedId:"how-to/collection-field-map",id:"how-to/collection-field-map",title:"Collection Field Map",description:"Asky support reusage of IAskyFieldMap definition",source:"@site/docs/how-to/collection-field-map.md",sourceDirName:"how-to",slug:"/how-to/collection-field-map",permalink:"/asky/docs/how-to/collection-field-map",draft:!1,editUrl:"https://github.com/webinex/asky/tree/main/docs/docs/how-to/collection-field-map.md",tags:[],version:"current",sidebarPosition:1,frontMatter:{sidebar_position:1},sidebar:"tutorialSidebar",previous:{title:"Supported Filters",permalink:"/asky/docs/support"},next:{title:"Child Field Map",permalink:"/asky/docs/how-to/child-field-map"}},c={},s=[],p={toc:s},d="wrapper";function u(e){let{components:t,...n}=e;return(0,i.kt)(d,(0,r.Z)({},p,n,{components:t,mdxType:"MDXLayout"}),(0,i.kt)("h1",{id:"collection-field-map"},"Collection Field Map"),(0,i.kt)("p",null,"Asky support reusage of ",(0,i.kt)("inlineCode",{parentName:"p"},"IAskyFieldMap<T>")," definition"),(0,i.kt)("pre",null,(0,i.kt)("code",{parentName:"pre",className:"language-csharp",metastring:'title="Parent.cs"',title:'"Parent.cs"'},"public class Parent\n{\n    public Guid Id { get; set; }\n    public string Name { get; set; }\n    public Child[] Childs { get; set; }\n}\n")),(0,i.kt)("pre",null,(0,i.kt)("code",{parentName:"pre",className:"language-csharp",metastring:'title="Child.cs"',title:'"Child.cs"'},"public class Child\n{\n    public Guid Id { get; set; }\n    public string Name { get; set; }\n}\n")),(0,i.kt)("pre",null,(0,i.kt)("code",{parentName:"pre",className:"language-csharp",metastring:'title="ParentFieldMap.cs',title:'"ParentFieldMap.cs'},'public class ParentFieldMap : IAskyFieldMap<Parent>\n{\n    public Expression<Func<Parent, object>> this[string fieldId] => fieldId switch\n    {\n        "name" => x => x.Name,\n        // Define collection\n        "child" => x => x.Childs,\n        // Use .Select() to defined collection fields\n        "child.name" => x => x.Childs.Select(c => c.Name),\n        _ => null,\n    };\n}\n')),(0,i.kt)("pre",null,(0,i.kt)("code",{parentName:"pre",className:"language-csharp",metastring:'title="ParentRepository.cs"',title:'"ParentRepository.cs"'},'public class ParentRepository\n{\n    private readonly IAskyFieldMap<Parent> _parentFieldMap;\n\n    // ...\n\n    public async Task<Parent[]> GetAllParentsWithChildJimAsync()\n    {\n        var filterRule = FilterRule.Any(\n            "child",\n            FilterRule.Eq("child.name", "Jim"));\n\n        // ...\n    }\n}\n')))}u.isMDXComponent=!0}}]);