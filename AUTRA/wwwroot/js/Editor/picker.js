class GPUPickHelper {
    constructor() {
        // create a 9x9 pixel render target
        this.pickingTexture = new THREE.WebGLRenderTarget(9, 9);
        //Create Array to store the  color (RGBA) of each pixel (4 elements per each pixel)
        this.pixelBuffer = new Uint8Array(4 * 81);
        this.pickedObject = null;
        this.selectedObject = new Set();
        this.tempSelected = new Set(); //get the area selected elements and move them to selectedObject
        this.emissiveFlash = 0xcc5511;
        this.objectIds = [];
    }
    recordObject(object, id) { //Record the object with its id in the objects array 
        this.objectIds[id] = object.visual;
    }

    renderPicking(position, width, height, renderer, pickingScene, camera, pickingTexture, pixelBuffer) {
        const pixelRatio = renderer.getPixelRatio();
        //move camera to the required area
        camera.setViewOffset(
            renderer.getContext().drawingBufferWidth,   // full width
            renderer.getContext().drawingBufferHeight,  // full top
            position.x * pixelRatio | 0,                // rect x
            position.y * pixelRatio | 0,                // rect y
            width,                                      // rect width
            height,                                     // rect height
        );

        // render the scene
        renderer.setRenderTarget(pickingTexture);
        renderer.render(pickingScene, camera);
        //Reset the settings of renderer and camera
        renderer.setRenderTarget(null);
        camera.clearViewOffset();

        //read the pixels colors
        renderer.readRenderTargetPixels(
            pickingTexture,
            0,              // x-offset
            0,              // y-offset
            width,          // width
            height,         // height
            pixelBuffer);
    }

    getObject(cssPosition, renderer, pickingScene, camera) {
        cssPosition.x -= 4;
        cssPosition.y -= 4;

        this.renderPicking(cssPosition, 9, 9, renderer, pickingScene, camera, this.pickingTexture, this.pixelBuffer);

        for (let i = 2; i < this.pixelBuffer.length; i += 4) {
            if (this.pixelBuffer[i]) { // Check if the Blue component has a value
                //Only the first colored pixel is needed
                return this.objectIds[this.pixelBuffer[i] | this.pixelBuffer[i - 1] << 8].mesh;
            }
        }

        return null;
    }

    //Trial
    getObjects(initialPosition, rectWidth, rectHeight, renderer, pickingScene, camera) {
        let pickingTexture = new THREE.WebGLRenderTarget(rectWidth, rectHeight);
        let pixelsBuffer = new Uint8Array(4 * rectWidth * rectHeight);

        this.renderPicking(initialPosition, rectWidth, rectHeight, renderer, pickingScene,
            camera, pickingTexture, pixelsBuffer);


        for (let i = 2; i < pixelsBuffer.length; i += 4) {
            if (pixelsBuffer[i]) { // Check if the Blue component has a value
                this.tempSelected.add(this.objectIds[pixelsBuffer[i] | pixelsBuffer[i - 1] << 8].mesh); //Combine Blue and Green Components
            }
        }
    }

    selectByArea(initialPosition, rectWidth, rectHeight, multiple, renderer, pickingScene, camera) { //On mouse click
        // restore the color if there is a picked object
        if (!multiple)
            this.unselect();

        this.getObjects(initialPosition, rectWidth, rectHeight, renderer, pickingScene, camera);

        for (let item of this.tempSelected) {
            item.material.color.setHex(item.material.color.getHex() + this.emissiveFlash);
            this.selectedObject.add(item);
        }
        this.tempSelected.clear();
    }

    select(cssPosition, multiple, renderer, pickingScene, camera) { //On mouse click
        // restore the color if there is a picked object
        if (!multiple)
            this.unselect();

        let object = this.getObject(cssPosition, renderer, pickingScene, camera, this.objectIds);

        if (object) {
            this.selectedObject.add(object);
            object.material.color.setHex(object.material.color.getHex() + this.emissiveFlash);
            if (object.userData.element) {
                let element = object.userData.element;
                $('#beamSection').val(element.visual.sectionName);
                $('#beamStart').val(`${object.position.x},${object.position.z},${object.position.y}`);
                $('#beamEnd').val(`${element.visual.endPoint.x},${element.visual.endPoint.z},${element.visual.endPoint.y}`);
                $('#beamDead').val(object.userData.element.data.lineLoads[0] ? object.userData.element.data.lineLoads[0].magnitude*-1 : 0);
                $('#beamLive').val(object.userData.element.data.lineLoads[1] ? object.userData.element.data.lineLoads[1].magnitude*-1 : 0);
            }
        }
    }

    unselect() {
        for (let item of this.selectedObject) {
            item.material.color.setHex(item.material.color.getHex() - this.emissiveFlash);
        }
        this.selectedObject.clear();
    }

    pick(cssPosition, renderer, pickingScene, camera, idToObject) { //On mouse hover
        // restore the color if there is a picked object
        if (this.pickedObject) {
            this.pickedObject.material.color.setHex(this.pickedObject.material.color.getHex() - this.emissiveFlash);
        }

        this.pickedObject = this.getObject(cssPosition, renderer, pickingScene, camera, idToObject);
        if (this.pickedObject) {
            this.pickedObject.material.color.setHex(this.pickedObject.material.color.getHex() + this.emissiveFlash);
        }
    }
}