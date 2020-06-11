let nodeGeometry = new THREE.SphereBufferGeometry(0.1, 32, 32);
let nodeMaterial = new THREE.MeshBasicMaterial({ color: 0xffcc00 });
let hingeMaterial = new THREE.MeshPhongMaterial({ color: 0x6633ff });
let hingeGeometry = new THREE.ConeBufferGeometry(0.3, 0.3, 4);
let id = 0;
class Node {
    constructor(coordX, coordY, coordZ, support) {
        this.data = {};
        this.data.$id = `${++id}`; //Metadata for JSON Referencing(to reference nodes in beams)
        this.data.support = support ?? 0;
        this.data.position = new THREE.Vector3(coordX, coordY, coordZ);  //TODO : Switch Y & Z?!	
        this.data.pointLoads = [];

        this.visual = {};
        if (this.data.support) {
            this.visual.mesh = new THREE.Line(hingeGeometry, hingeMaterial.clone());
            this.visual.mesh.position.set(coordX, (coordY - 0.15), coordZ);
        }
        else {
            this.visual.mesh = new THREE.Mesh(nodeGeometry, nodeMaterial.clone());
            this.visual.mesh.position.set(coordX, coordY, coordZ);
        }
        this.visual.mesh.userData.node = this;
    }
    addLoad(load, replace) {
        let index = this.data.pointLoads.findIndex(l => l.pattern === load.pattern);
        if (index < 0) { //has no load of the same case(pattern)	
            this.data.pointLoads.push(load);
            index = this.data.pointLoads.length - 1;
        }
        else if (replace) //has a load of the same case(pattern) , Replace it	
            this.data.pointLoads[index] = load;
        else              //has a load of the same case(pattern) , Add to it	
            this.data.pointLoads[index].magnitude += load.magnitude;
        return index;
    }
    showReaction(pattern) {
        let reaction = this.visual.reactions.find(r => r.pattern == pattern);
        let position = this.visual.mesh.position.clone();
        position.y -= 1.15;
        let arrow = new THREE.ArrowHelper(new THREE.Vector3(0, 1, 0), position, 1, 0xcc00ff);
        let textGeometry = new THREE.TextBufferGeometry(`${reaction.rv}`, {
            font: myFont,
            size: 0.2,
            height: 0,
            curveSegments: 3,
            bevelEnabled: false
        });
        let text = new THREE.Mesh(textGeometry, fontMaterial);
        arrow.add(text);
        return arrow;
    }
    static create(coordX, coordY, coordZ, support, editor, nodes) { //Static method to handle node creation, 
        let node = new Node(coordX, coordY, coordZ, support);       // recording, visualization 
        nodes.push(node);
        editor.addToGroup(node.visual.mesh, 'nodes');
        editor.createPickingObject(node);
        return node;
    }
}

//Naming with X or Z denotes the outer loop
function createNodesX(editor, coordX, coordZ) {
    let nodes = [];
    for (let i = 0; i < coordX.length; i++) {
        for (let j = 0; j < coordZ.length; j++) {
            Node.create(coordX[i], 0, coordZ[j], 'Hinge', editor, nodes);
        }
    }
    return nodes;
}

function createNodesZ(editor, coordX, coordZ) {
    let nodes = [];
    for (let i = 0; i < coordZ.length; i++) {
        for (let j = 0; j < coordX.length; j++) {
            Node.create(coordX[j], 0, coordZ[i], 'Hinge', editor, nodes);
        }
    }
    return nodes;
}