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
        this.currentId = 0; //to match objects with correspondents in picking scene
        this.canvas;
    }
    init(coordX, coordZ) {
        //#region Creating camera
        this.camera.position.set(0.5 * coordX, 20, 2 * coordZ);
        this.camera.lookAt(new THREE.Vector3(0.5 * coordX, 0, 0.5 * coordZ)); //looks at the middle of the model
        //#endregion

        //#region Renderer
        this.renderer.setClearColor(0xdddddd); //setting color of canvas
        this.canvas = this.renderer.domElement;
        this.renderer.setSize(window.innerWidth, window.innerHeight); //setting width and height of canvas(canvas.width, canvas.height)
        //#endregion

        //#region Controls
        let orbitControls = new THREE.OrbitControls(this.camera, this.renderer.domElement);
        orbitControls.mouseButtons = { // Set the functions of mouse buttons
            LEFT: THREE.MOUSE.ROTATE,
            MIDDLE: THREE.MOUSE.PAN,
            RIGHT: THREE.MOUSE.ROTATE
        };
        orbitControls.target.set(0.5 * coordX, 0, 0.5 * coordZ); //Set the target to the camera lookAt
        orbitControls.update();
        //#endregion

        //#region Light
        let directionalLight = new THREE.DirectionalLight(0xffffff, 1);
        directionalLight.position.set(0, 5, 3);
        this.scene.add(directionalLight);
        //#endregion

        //Collect similar objects in groups
        this.scene.userData.elements = new THREE.Group();
        this.scene.add(this.scene.userData.elements);
        this.scene.userData.nodes = new THREE.Group();
        this.scene.add(this.scene.userData.nodes);
        this.scene.userData.grids = new THREE.Group();
        this.scene.add(this.scene.userData.grids);
        this.scene.userData.dimensions = new THREE.Group();
        this.scene.add(this.scene.userData.dimensions);
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
    addToGroup(object, type) { //add object to one of the created groups(elements,nodes.....)
        this.scene.userData[type].add(object);
    }
    removeFromGroup(object, type) { //remove object from one of the created groups
        this.scene.userData[type].remove(object);
        object.geometry.dispose();
        object.material.dispose();
        if (object.userData.picking) {
            this.pickingScene.remove(object.userData.picking)
            object.userData.picking.material.dispose();
            object.userData.picking.geometry.dispose();
        }
    }
    addToScene(object) { //add to scene directly
        this.scene.add(object);
    }
    removeFromScene(object) {//remove from scene directly
        this.scene.remove(object);
        object.geometry.dispose();
        object.material.dispose();
    }
    createPickingObject(object) { //Create a picking object for the parameter
        this.pickingScene.add(pickingObject(object, ++this.currentId));
        this.picker.recordObject(object, this.currentId);
    }
    toggleBeams() { //Toggle view between wireframe and 3D
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
    pick(event) { //Highlights object on hover
        this.picker.pick(this.setPickPosition(event), this.renderer, this.pickingScene, this.camera);
    }
    select(event, multiple) { //Select single object on click
        this.picker.select(this.setPickPosition(event), multiple, this.renderer, this.pickingScene, this.camera);
    }
    selectByArea(initialPosition, rectWidth, rectHeight, multiple) {//Select multiple objects by area (hold and drag mouse)
        this.picker.selectByArea(initialPosition, rectWidth, rectHeight, multiple, this.renderer, this.pickingScene, this.camera)
    }
    clearGroup(group) { //Clear the components of a group
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
    hideGroup(groupName) { //Temporarily hide a group from scene
        let group = this.scene.userData[groupName];
        if (this.scene.children.includes(group))
            this.scene.remove(group);
    }
    showGroup(groupName) {//show the temporarily hiden group from scene
        let group = this.scene.userData[groupName];
        if (!this.scene.children.includes(group))
            this.scene.add(group);
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
    screenshot() { //Take a screenshot to the view
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
    getIntersected(position) { //get the object at the given position (world position)
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