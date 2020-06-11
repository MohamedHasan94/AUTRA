//controllers & commands
function pickingObject(object, id) { //The object in picking scene used to pick the original object (GPU Picking)
    let material = new THREE.LineBasicMaterial({ color: new THREE.Color(id) });
    let mesh = new THREE.Line(object.visual.mesh.geometry, material);
    mesh.position.copy(object.visual.mesh.position);
    mesh.rotation.copy(object.visual.mesh.rotation);
    object.visual.mesh.userData.picking = mesh; // The object has a reference to its picking object
    return mesh;
}

class Editor {
    constructor() {
        this.scene = new THREE.Scene();
        this.pickingScene = new THREE.Scene();
        this.picker = new GPUPickHelper();
        this.pickingScene.background = new THREE.Color(0);
        this.camera = new THREE.PerspectiveCamera(45, window.innerWidth / window.innerHeight, 0.1, 5000);
        this.renderer = new THREE.WebGLRenderer({ antialias: true, canvas: document.getElementById('canvas') });
        this.currentId = 0;
        this.canvas;
    }
    init(sections) {
        this.picker.sections = sections;
        //#region Creating camera
        this.camera.position.set(0, 35, 70);
        this.camera.lookAt(this.scene.position); //looks at origin(0,0,0)
        //#endregion

        //#region Renderer
        this.renderer.setClearColor(0xdddddd); //setting color of canvas
        this.canvas = this.renderer.domElement;
        this.renderer.setSize(window.innerWidth, window.innerHeight); //setting width and height of canvas(canvas.width, canvas.height)
        //document.body.appendChild(this.canvas); //append canvas tag to html
        //#endregion

        //#region Controls
        let orbitControls = new THREE.OrbitControls(this.camera, this.renderer.domElement);
        orbitControls.mouseButtons = { // Set the functions of mouse buttons
            LEFT: THREE.MOUSE.ROTATE,
            MIDDLE: THREE.MOUSE.PAN,
            RIGHT: THREE.MOUSE.ROTATE
        };
        //#endregion

        //#region Light
        let directionalLight = new THREE.DirectionalLight(0xffffff, 1);
        directionalLight.position.set(0, 5, 3);
        this.scene.add(directionalLight);
        //#endregion

        //#region Creating Axess
        let axes = new THREE.AxesHelper(5);
        axes.position.set(-10, 0, 10);
        this.scene.add(axes);
        //#endregion

        //Collect similar objects in groups
        this.scene.userData.elements = new THREE.Group();
        this.scene.add(this.scene.userData.elements);
        this.scene.userData.nodes = new THREE.Group();
        this.scene.add(this.scene.userData.nodes);
        this.scene.userData.grids = new THREE.Group();
        this.scene.add(this.scene.userData.grids);
        this.scene.userData.loads = new THREE.Group();
        this.scene.add(this.scene.userData.loads);
        this.scene.userData.results = new THREE.Group();
        this.scene.add(this.scene.userData.results);

        this.loop();
    }
    loop() {
        this.renderer.render(this.scene, this.camera);
        requestAnimationFrame(() => this.loop());
    }
    addToGroup(object, type) {
        this.scene.userData[type].add(object);
    }
    removeFromGroup(object, type) {
        this.scene.userData[type].remove(object);
        object.geometry.dispose();
        object.material.dispose();
        if (object.userData.picking) {
            this.pickingScene.remove(object.userData.picking)
            object.userData.picking.material.dispose();
            object.userData.picking.geometry.dispose();
        }
    }
    addToScene(object) {
        this.scene.add(object);
    }
    removeFromScene(object) {
        this.scene.remove(object);
        object.geometry.dispose();
        object.material.dispose();
    }
    createPickingObject(object) {
        this.pickingScene.add(pickingObject(object, ++this.currentId));
        this.picker.recordObject(object, this.currentId);
    }
    toggleBeams() {
        let elements = this.scene.userData.elements;
        let length = elements.children.length;
        let visual;
        for (let i = 0; i < length; i++) {
            visual = elements.children[i].userData.element.visual;
            visual.temp = elements.children[i];

            visual.unusedMesh.position.copy(elements.children[i].position)
            visual.unusedMesh.rotation.copy(elements.children[i].rotation)

            elements.children[i] = visual.unusedMesh;
            visual.mesh = elements.children[i];

            visual.unusedMesh = visual.temp;
        }
    }
    setPickPosition(event) { //get the mouse position relative to the canvas (not the screen)
        const rect = this.canvas.getBoundingClientRect();
        //GPUPicker reads the pixels from the top left corner of the canvas
        return {
            x: (event.clientX - rect.left) * this.canvas.width / rect.width,
            y: (event.clientY - rect.top) * this.canvas.height / rect.height
        };
    }
    pick(event) {
        this.picker.pick(this.setPickPosition(event), this.renderer, this.pickingScene, this.camera);
    }
    select(event, multiple) {
        this.picker.select(this.setPickPosition(event), multiple, this.renderer, this.pickingScene, this.camera);
    }
    selectByArea(initialPosition, rectWidth, rectHeight, multiple) {
        this.picker.selectByArea(initialPosition, rectWidth, rectHeight, multiple, this.renderer, this.pickingScene, this.camera)
    }
    clearGroup(group) {
        group = this.scene.userData[group];
        let length = group.children.length;
        for (let i = 0; i < length; i++) {
            if (!group.children[i].children) { //if the object has its own children
                group.children[i].geometry.dispose();
                group.children[i].material.dispose();
            }
            else {
                group.children[i].children.forEach(c => {
                    c.material.dispose();
                    c.geometry.dispose();
                })
            }
        }
        group.children = [];
    }
    darkTheme = function () {
        this.renderer.setClearColor(0x000000);
        light.style.display = 'block';
        dark.style.display = 'none';
        this.changeColor(0xffff00, 0x0000ff, 0xff0000);
    }
    lightTheme = function () {
        this.renderer.setClearColor(0xdddddd);
        dark.style.display = 'block';
        light.style.display = 'none';
        this.changeColor(0x000000, 0xffcc00, 0x6633ff);
    }
    changeColor(elementsColor, nodesColor, supportsColor) {
        let elements = this.scene.userData.elements.children;
        for (let i = 0; i < elements.length; i++) {
            elements[i].material.color.setHex(elementsColor);
        }

        let nodes = this.scene.userData.nodes.children;
        for (let i = 0; i < nodes.length; i++) {
            if (nodes[i] instanceof THREE.Mesh) //Normal Node
                nodes[i].material.color.setHex(nodesColor);
            else //Support
                nodes[i].material.color.setHex(supportsColor);
        }
    }
    screenshot() {
        let imgData;
        try {
            this.renderer.render(this.scene, this.camera);
            imgData = this.canvas.toDataURL("image/jpg");
            imgData.replace("image/jpg", "image/octet-stream");

        } catch (e) {
            console.log(e);
            return;
        }

        let link = document.createElement('a'); //Create HTML link to download the file on client machine
        link.setAttribute('download', 'screen.jpg');
        link.href = imgData;
        document.body.appendChild(link);

        setTimeout(function () { // domElement takes some time to be added to the document
            link.click(); //Fire the click event of the link
            document.body.removeChild(link); //The link is no longer needed
        }, 1000);
    }
    getIntersected(position) {
        let widthHalf = window.innerWidth / 2, heightHalf = window.innerHeight / 2;
        position.project(this.camera); //Project the 3D world position on the screen
        //The resulting position is between[-1,1] WebGl coordinates with the origin at the screen center
        //Switch position to screen position with the origin at the top left corner
        position.x = (position.x * widthHalf) + widthHalf;
        position.y = - (position.y * heightHalf) + heightHalf;
        //remove nodes from picking pickingScene
        let temp = this.pickingScene.children;
        this.pickingScene.children = this.scene.userData.elements.children.map(c => c.userData.picking);
        //Read the position using the GPUPicker
        let pickeckedMesh = this.picker.getObject(position, this.renderer, this.pickingScene, this.camera);
        this.pickingScene.children = temp;
        return pickeckedMesh;
    }
}