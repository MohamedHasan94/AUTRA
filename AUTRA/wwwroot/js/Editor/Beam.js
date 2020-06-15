let vector = new THREE.Vector3();
let zVector = new THREE.Vector3(0, 0, 1);
let shapes = [THREE.Path, THREE.Shape];
let meshes = [THREE.Line, THREE.Mesh];
let materials = [new THREE.LineBasicMaterial({ color: 0x0000ff }), new THREE.MeshBasicMaterial({ color: 0x0000ff, transparent: true, opacity: 0.7, side: THREE.DoubleSide })];

class Beam extends FrameElement {
    constructor(section, startPoint, endPoint, shape, lineMaterial, meshMaterial, startNode, endNode) {
        let direction = (vector.clone().subVectors(endPoint, startPoint)).normalize();
        let rotation = new THREE.Euler(0, direction.angleTo(zVector), 0);
        super(section, startPoint, endPoint, shape, lineMaterial, meshMaterial, startNode, endNode, direction, rotation);
        //this.data.span = endPoint.distanceTo(startPoint);
        this.data.innerNodes = [];
        this.visual.mesh.userData.element = this;
    }
    clone() { //Create a copy of this instance
        return new Beam({ $id: this.data.section.$ref, name: this.visual.sectionName }, this.visual.mesh.position.clone(),
            this.visual.endPoint.clone(), this.visual.extruded.geometry.parameters.shapes,
            lineMaterial.clone(), meshMaterial.clone(), this.startNode, this.endNode);
    }
    addLoad(load, replace) {
        let index = this.data.lineLoads.findIndex(l => l.pattern === load.pattern);
        if (index < 0) { //has no load of the same case(pattern)
            this.data.lineLoads.push(load.clone());
            index = this.data.lineLoads.length - 1;
        }
        else if (replace) //has a load of the same case(pattern) , Replace it
            this.data.lineLoads[index] = load.clone();
        else              //has a load of the same case(pattern) , Add to it
            this.data.lineLoads[index].magnitude += load.magnitude;
        return index;
    }
    showMoment(pattern, display, domEvents) {
        let stations = this.visual.strainingActions.find(sa => sa.pattern == pattern).stations;
        let half = parseInt(0.5 * stations.length);

        let scale = -1 * (0.095 * stations[half].mo + 0.185);//(1 --> 0.25, 20 --> 2)
        scale = scale < -2 ? -2 : scale > -0.25 ? -0.25 : scale; //Check that scale is in boundries
        scale /= stations[half].mo; // Get the ratio(will be multiplied by the value)

        let shape = new shapes[display]();
        if (this.data.innerNodes.length == 0) {
            shape.quadraticCurveTo(stations[half].x, stations[half].mo * scale*2, this.data.length, stations[stations.length - 1].mo * scale);
        }
        else {
            for (let i = 1; i < stations.length; i++) {
                shape.lineTo(stations[i].x, stations[i].mo * scale);
            }
        }
        let geometry;
        if (display) {
            geometry = new THREE.ShapeBufferGeometry(shape);
        }
        else {
            geometry = new THREE.BufferGeometry().setFromPoints(shape.getPoints());
        }
        let mesh = new meshes[display](geometry, materials[display]);
        this.createresultNodes(stations, scale, domEvents, mesh, 'mo','t.m', '');
        mesh.rotation.copy(this.visual.mesh.rotation);
        mesh.rotation.y -= 0.5 * Math.PI;
        mesh.position.copy(this.visual.mesh.position);
        return mesh;
    }
    showShear(pattern, display, domEvents) {
        let stations = this.visual.strainingActions.find(sa => sa.pattern == pattern).stations;

        let scale = 0.06 * stations[0].vo + 0.19;
        scale = scale > 2 ? 2 : scale < 0.25 ? 0.25 : scale;//Check that scale is in boundries
        scale /= stations[0].vo;// Get the ratio(will be multiplied by the value)
                
        let shape1 = new shapes[display]();
        let shape2 = shape1.clone();
        
        let half = parseInt(0.5 * stations.length);
        for (let i = 0; i < half; i++) {
            shape1.lineTo(stations[i].x, stations[i].vo * scale);
            
            shape1.lineTo(stations[i].x, stations[i].vf * scale);
        }
        shape1.lineTo(stations[half].x, stations[half].vo * scale);

        shape2.moveTo(stations[half].x, 0);
        for (let i = half; i < stations.length; i++) {
            shape2.lineTo(stations[i].x, stations[i].vo * scale);
            shape2.lineTo(stations[i].x, stations[i].vf * scale);
        }
        shape2.lineTo(stations[stations.length - 1].x, 0);

        let geometry;
        if (display) {
            let geometry1 = new THREE.ShapeBufferGeometry(shape1);
            let geometry2 = new THREE.ShapeBufferGeometry(shape2);
            geometry = THREE.BufferGeometryUtils.mergeBufferGeometries([geometry1, geometry2]);
        }
        else {
            let geometry1 = new THREE.BufferGeometry().setFromPoints(shape1.getPoints());
            let geometry2 = new THREE.BufferGeometry().setFromPoints(shape2.getPoints());
            geometry = THREE.BufferGeometryUtils.mergeBufferGeometries([geometry1, geometry2]);
        }

        let mesh = new meshes[display](geometry, materials[display]);

        this.createresultNodes(stations, scale, domEvents, mesh, 'vo', 't', 'vf');
        mesh.rotation.copy(this.visual.mesh.rotation);
        mesh.rotation.y -= 0.5 * Math.PI;
        mesh.position.copy(this.visual.mesh.position);
        return mesh;
    }
    static switchType(beam, type1, type2) {//Switches the beam from main to secondary and vice versa
        let beamIndex = type1.indexOf(beam);
        if (beamIndex > -1) {
            type1.splice(beamIndex, 1);
            type2.unshift(beam);
        }
    }
    static generate(beams, editor) {
        let generatedBeams = [];
        let previousSection = beams[0].section.name;
        let dimensions = new SectionDimensions(parseInt(previousSection.split('E')[1]) / 1000);
        let shape = createShape(dimensions);
        for (let i = 0; i < beams.length; i++) {
            if (beams[i].section.name !== previousSection) {
                dimensions = new SectionDimensions(parseInt(beams[i].section.name.split('E')[1]) / 1000);
                shape = createShape(dimensions);
            }
            let beam = new Beam(beams[i].section, beams[i].startNode.data.position, beams[i].endNode.data.position,
                shape, lineMaterial.clone(), meshMaterial.clone(), beams[i].startNode, beams[i].endNode);
            for (let j = 0; j < beams[i].innerNodes.length; j++) {
                beam.data.innerNodes.push({ $ref: beams[i].innerNodes[j].$id })
            }

            for (let j = 0; j < beams[i].lineLoads.length; j++) {
                beam.data.lineLoads.push(new LineLoad(beams[i].lineLoads[j].magnitude, beams[i].lineLoads[j].pattern))
            }            
            generatedBeams.push(beam);
            editor.addToGroup(beam.visual.mesh, 'elements');
            editor.createPickingObject(beam);
        }
        return generatedBeams;
    }
}

//Calculate the starting coords of secondary beams
function getSecCoords(mainCoord, secSpacing) {
    let coord = [0], number, sum = 0;
    number = mainCoord.length;

    for (var i = 1; i < number; i++) {
        while (sum < mainCoord[i]) {
            sum = 10 * sum + 10 * (secSpacing[i - 1] ?? secSpacing[0]);
            sum /= 10;
            coord.push(sum);
        }
    }
    return coord;
}

//Automatically generate the floor system from user's input with main beams on Z-axis (Creates the nodes with the beams)
function generateMainBeamsZ(editor, coordX, coordY, coordZ, mainSection, secSection, secSpacing) {
    let mainBeams, secBeams, secCoord = [0], secNodes, nodes;

    [mainBeams, nodes] = createZBeamsWithNodes(editor, coordX, coordY, coordZ, mainSection); //Create main beams on z-axis

    secCoord = getSecCoords(coordZ, secSpacing);
    [secBeams, secNodes] = createXBeams(editor, coordX, coordY, secCoord, secSection, coordZ, nodes, mainBeams);  //Create secondary beams on x-axis

    return [mainBeams, secBeams, nodes, secNodes];
}

//Automatically generate the floor system from user's input with main beams on X-axis (Creates the nodes with the beams)
function generateMainBeamsX(editor, coordX, coordY, coordZ, mainSection, secSection, secSpacing) {
    let mainBeams, secBeams, secCoord = [0], secNodes, nodes;

    [mainBeams, nodes] = createXBeamsWithNodes(editor, coordX, coordY, coordZ, mainSection); //Create main beams on x-axis (short direction)

    secCoord = getSecCoords(coordX, secSpacing);
    [secBeams, secNodes] = createZBeams(editor, secCoord, coordY, coordZ, secSection, coordX, nodes, mainBeams);  //Create secondary beams on z-axis (long direction)

    return [mainBeams, secBeams, nodes, secNodes];
}

//Create one material and clone from it (better performance)
let lineMaterial = new THREE.LineBasicMaterial({ color: 0x000000 });
let meshMaterial = new THREE.MeshPhongMaterial({ color: 0x0000ff });

//Secondary beams on X (Uses the existing nodes and creates the intermediate nodes)
function createXBeams(editor, coordX, coordY, coordZ, section, coordZToCheck, nodes, mainBeams) {
    let beams = [];
    let secBeamsNodes = [];
    let dimensions = new SectionDimensions(parseInt(section.name.split('E')[1]) / 1000);
    let shape = createShape(dimensions);

    let m = 0, a = -1, e = 1, createNode = false;
    let node1, node2;
    for (let i = 0; i < coordZ.length; i++) {
        if (coordZToCheck[e] / coordZ[i] != 1 && coordZ[i]) {
            //Craete a start node for this line
            m++;
            node2 = Node.create(coordX[0], coordY, coordZ[i], null, editor, secBeamsNodes);
            mainBeams[(i - m)].data.innerNodes.push({ "$ref": node2.data.$id });
            createNode = true;
        }
        else {
            a++;
            createNode = false;
        }

        for (let j = 0; j < coordX.length - 1; j++) {
            let beam;
            if (createNode) {
                node1 = node2;
                node2 = Node.create(coordX[j + 1], coordY, coordZ[i], null, editor, secBeamsNodes);

                beam = new Beam(section, node1.data.position.clone(), node2.data.position.clone(),
                    shape, lineMaterial.clone(), meshMaterial.clone(), node1, node2);

                mainBeams[((coordZToCheck.length - 1) * (j + 1)) + (i - m)].data.innerNodes.push({ "$ref": node2.data.$id });
            }
            else {
                beam = new Beam(section, new THREE.Vector3(coordX[j], coordY, coordZ[i]), new THREE.Vector3(coordX[j + 1], coordY, coordZ[i]),
                    shape, lineMaterial.clone(), meshMaterial.clone(), nodes[coordZToCheck.length * j + a], nodes[coordZToCheck.length * (j + 1) + a]);
            }
            beams.push(beam);
            editor.addToGroup(beam.visual.mesh, 'elements');
            editor.createPickingObject(beam);
        }
        e += parseInt(coordZ[i] / coordZToCheck[e]); //Check if entered the next main spacing
    }
    return [beams, secBeamsNodes];
}

//Secondary beams on Z(Uses the existing nodes and creates the intermediate nodes)
function createZBeams(editor, coordX, coordY, coordZ, section, coordXToCheck, nodes, mainBeams) {
    let beams = [];
    let secBeamsNodes = [];
    let dimensions = new SectionDimensions(parseInt(section.name.split('E')[1]) / 1000);
    let shape = createShape(dimensions);

    let m = 0, a = -1, e = 1, createNode = false;
    let node1, node2;
    for (let i = 0; i < coordX.length; i++) {
        if (coordXToCheck[e] / coordX[i] != 1 && coordX[i]) {
            //Craete a start node for this line
            m++;
            node2 = Node.create(coordX[i], coordY, coordZ[0], null, editor, secBeamsNodes);
            mainBeams[(i - m)].data.innerNodes.push({ "$ref": node2.data.$id });
            createNode = true;
        }
        else {
            a++;
            createNode = false;
        }

        for (let j = 0; j < coordZ.length - 1; j++) {
            let beam;
            if (createNode) {
                node1 = node2;
                node2 = Node.create(coordX[i], coordY, coordZ[j + 1], null, editor, secBeamsNodes);

                beam = new Beam(section, node1.data.position.clone(), node2.data.position.clone(),
                    shape, lineMaterial.clone(), meshMaterial.clone(), node1, node2);

                mainBeams[((coordXToCheck.length - 1) * (j + 1)) + (i - m)].data.innerNodes.push({ "$ref": node2.data.$id });
            }
            else {
                beam = new Beam(section, new THREE.Vector3(coordX[i], coordY, coordZ[j]), new THREE.Vector3(coordX[i], coordY, coordZ[j + 1]),
                    shape, lineMaterial.clone(), meshMaterial.clone(), nodes[coordXToCheck.length * j + a], nodes[coordXToCheck.length * (j + 1) + a]);
            }
            beams.push(beam);
            editor.addToGroup(beam.visual.mesh, 'elements');
            editor.createPickingObject(beam);
        }
        e += parseInt(coordX[i] / coordXToCheck[e]); //Check if entered the next main spacing
    }
    return [beams, secBeamsNodes];
}

//Main beams on Z (Creates both beams and nodes)
function createZBeamsWithNodes(editor, coordX, coordY, coordZ, section) {
    let beams = [];
    let dimensions = new SectionDimensions(parseInt(section.name.split('E')[1]) / 1000);
    let shape = createShape(dimensions);

    let nodes = [];
    let k = 0;

    for (let i = 0; i < coordX.length; i++) {
        Node.create(coordX[i], coordY, coordZ[0], null, editor, nodes);
        k++;

        for (let j = 0; j < coordZ.length - 1; j++) {
            Node.create(coordX[i], coordY, coordZ[j + 1], null, editor, nodes);

            let beam = new Beam(section, new THREE.Vector3(coordX[i], coordY, coordZ[j]), new THREE.Vector3(coordX[i], coordY, coordZ[j + 1]),
                shape, lineMaterial.clone(), meshMaterial.clone(), nodes[k - 1], nodes[k]);
            beams.push(beam);
            editor.addToGroup(beam.visual.mesh, 'elements');
            editor.createPickingObject(beam);
            k++;
        }
    }
    return [beams, nodes];
}

//Main beams on X (Creates both beams and nodes)
function createXBeamsWithNodes(editor, coordX, coordY, coordZ, section) {
    let beams = [];
    let dimensions = new SectionDimensions(parseInt(section.name.split('E')[1]) / 1000);
    let shape = createShape(dimensions);

    let nodes = [];
    let k = 0;
    for (let i = 0; i < coordZ.length; i++) {
        Node.create(coordX[0], coordY, coordZ[i], null, editor, nodes);
        k++;

        for (let j = 0; j < coordX.length - 1; j++) {
            Node.create(coordX[j + 1], coordY, coordZ[i], null, editor, nodes);

            let beam = new Beam(section, new THREE.Vector3(coordX[j], coordY, coordZ[i]), new THREE.Vector3(coordX[j + 1], coordY, coordZ[i]),
                shape, lineMaterial.clone(), meshMaterial.clone(), nodes[k - 1], nodes[k]);
            beams.push(beam);
            editor.addToGroup(beam.visual.mesh, 'elements');
            editor.createPickingObject(beam);
            k++;
        }

    }
    return [beams, nodes];
}

function drawBeamByTwoPoints(section, startNode, endNode) {
    let dimensions = new SectionDimensions(parseInt(section.name.split('E')[1]) / 1000);
    let shape = createShape(dimensions);
    return new Beam(section, startNode.visual.mesh.position.clone(), endNode.visual.mesh.position.clone(), shape,
        lineMaterial.clone(), meshMaterial.clone(), startNode, endNode);
}