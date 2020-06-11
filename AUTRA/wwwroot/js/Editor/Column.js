class Column extends FrameElement {
    constructor(section, startPoint, endPoint, shape, lineMaterial, meshMaterial, startNode, endNode) {
        let direction = (vector.clone().subVectors(endPoint, startPoint)).normalize();
        let rotation = new THREE.Euler(-1 * direction.angleTo(zVector), 0, 0);
        super(section, startPoint, endPoint, shape, lineMaterial, meshMaterial, startNode, endNode, direction, rotation);
        this.visual.mesh.userData.element = this;
    }
    clone() { //Create a copy of this instance
        return new Column({ $id: this.data.section.$ref, name: this.visual.sectionName }, this.visual.mesh.position.clone(), this.visual.endPoint.clone(), this.visual.extruded.geometry.parameters.shapes,
            lineMaterial.clone(), meshMaterial.clone(), this.startNode, this.endNode);
    }
}

function generateColumnsX(editor, coordX, coordZ, mainNodesA, mainNodesB, section) {
    let columns = [];
    let xNo = coordX.length, zNo = coordZ.length;
    let dimensions = new SectionDimensions(parseInt(section.name.split(' ')[1]) / 1000);
    let shape = createShape(dimensions);
    for (let i = 0; i < xNo; i++) {

        for (let j = 0; j < zNo; j++) {
            let column = new Column(section, mainNodesA[i * zNo + j].data.position.clone(), mainNodesB[i * zNo + j].data.position.clone(),
                shape, lineMaterial.clone(), meshMaterial.clone(), mainNodesA[i * zNo + j], mainNodesB[i * zNo + j]);

            columns.push(column);
            editor.addToGroup(column.visual.mesh, 'elements');
            editor.createPickingObject(column);
        }

    }
    return columns;
}

function generateColumnsZ(editor, coordX, coordZ, mainNodesA, mainNodesB, section) {
    let columns = [];
    let xNo = coordX.length, zNo = coordZ.length;
    let dimensions = new SectionDimensions(parseInt(section.name.split(' ')[1]) / 1000);
    let shape = createShape(dimensions);
    for (let i = 0; i < zNo; i++) {

        for (let j = 0; j < xNo; j++) {

            let column = new Column(section, mainNodesA[i * xNo + j].data.position.clone(), mainNodesB[i * xNo + j].data.position.clone(),
                shape, lineMaterial.clone(), meshMaterial.clone(), mainNodesA[i * xNo + j], mainNodesB[i * xNo + j]);

            columns.push(column);
            editor.addToGroup(column.visual.mesh, 'elements');
            editor.createPickingObject(column);
        }

    }
    return columns;
}

function drawColumnByTwoPoints(section, startNode, endNode) {
    let dimensions = new SectionDimensions(parseInt(section.name.split(' ')[1]) / 1000);
    let shape = createShape(dimensions);
    return new Column(section, startNode.data.position.clone(), endNode.data.position.clone(), shape,
        lineMaterial.clone(), meshMaterial.clone(), startNode, endNode);
}