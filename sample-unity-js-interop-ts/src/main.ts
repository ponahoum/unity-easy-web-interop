import './style.css'
import { RuntimeWebTests, System, UnityInstance } from './testing/testing';

// Create canvas on the page
const buildUrl = "build/Build/";
var loaderUrl = buildUrl + "/build.loader.js";
const canvas = document.createElement('canvas');
// Add canvas to app
document.body.appendChild(canvas);
canvas.width = 800;
canvas.height = 600;


// Set canvas width height by style
canvas.style.width = "800px";
canvas.style.height = "600px";

// name canvas #unity-canvas
canvas.id = "unity-canvas";


var config = {
    dataUrl: buildUrl + "/build.data.gz",
    frameworkUrl: buildUrl + "/build.framework.js.gz",
    codeUrl: buildUrl + "/build.wasm.gz",
    streamingAssetsUrl: "StreamingAssets",
    companyName: "DefaultCompany",
    productName: "Test WASM",
    productVersion: "0.1",
    showBanner: false,
};


var script = document.createElement("script");
script.src = loaderUrl;
script.onload = () => {
    createUnityInstance(canvas, config, (progress: number) => {
        console.log(progress);
    }).then((unityInstance: UnityInstance) => {
        console.log(unityInstance);
        var module = unityInstance.Module;
        var instanceOfRuntimeWebTests: RuntimeWebTests = module.static.RuntimeWebTests.GetNewTestInstance();
        var aDouble: System.Double = instanceOfRuntimeWebTests.MyMethodReturningDouble();
        console.log((aDouble as any).value);
        var addedOne = instanceOfRuntimeWebTests.AddOneToDouble(aDouble);
        console.log((addedOne as any).value);
        //instanceOfRuntimeWebTests.ConcatenateStrings(anInt, anInt);

    }).catch((message) => {
        alert(message);
    });
};

document.body.appendChild(script);

//var consoleService = Module.exposed.System.Console;
//const uu = new MyClassExample();
//MyClassExample.GetBranch();
//console.log(consoleService.GetBranch());
//Module.exposed.SystemD.Bruh.GetBranch(uu);
