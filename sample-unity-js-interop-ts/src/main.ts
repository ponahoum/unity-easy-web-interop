import { UnityInstance } from './UnityJSInterop';
import './style.css'
import { RunAllTests } from './tests/run-all-tests';

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
    }).then(async (unityInstance: UnityInstance) => {
        RunAllTests(unityInstance);
    }).catch((message: any) => {
        alert(message);
    });
};

document.body.appendChild(script);