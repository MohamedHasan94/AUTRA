function Grid(spaceX, spaceZ, shift, levels) {
    //coordinates in x-direction
    this.coordX = spaceX;//[0,5,10,15,20]
    //coordinates in z-direction
    this.coordZ = spaceZ;//[0,6,12,18]
    //levels
    this.levels = levels;

    let numberInX = this.coordX.length;
    let numberInZ = this.coordZ.length;


    //Variable spacing:
    this.lineLengthInX = spaceX[numberInX - 1] + 2 * shift;
    this.lineLengthInZ = spaceZ[numberInX - 1] + 2 * shift;

    //Create a Group of grid lines
    this.linesInX = new THREE.Group();
    this.linesInZ = new THREE.Group();

    let xDirection = new THREE.Vector3(1, 0, 0);
    let zDirection = new THREE.Vector3(0, 0, 1);

    
    //Fill the horizontal Group 
    for (let i = 0; i < numberInX; i++) {
        this.linesInX.add((new Line(new THREE.Vector3(spaceX[i], 0, -shift), new THREE.Vector3(spaceX[i], 0, - shift + this.lineLengthInZ),
            this.lineLengthInZ, zDirection)).line);
    }
    //Fill the vertical Group 
    for (let i = 0; i < numberInZ; i++) {
        this.linesInZ.add((new Line(new THREE.Vector3(-shift, 0, spaceZ[i]), new THREE.Vector3(-shift + this.lineLengthInX, 0, spaceZ[i]),
            this.lineLengthInX, xDirection)).line);
    }
    
    //////////////////////////////////////////////////////
    let xLength = this.lineLengthInX;
    let zLength = this.lineLengthInZ;
    let matrix = new THREE.Matrix4();
    let geoProp = {
        font: myFont,
        size: 0.75,
        height: 0,
        curveSegments: 3,
        bevelEnabled: false
    };
    let mergedGeometry = new THREE.TextBufferGeometry(``, geoProp);
    for (let i = 0; i < numberInX; i++) {
        let geometry = new THREE.TextBufferGeometry(`${String.fromCharCode(i + 65)}`, geoProp);
        let geometry2 = geometry.clone();
        geometry.applyMatrix4(matrix.makeRotationX(Math.PI / 2)).applyMatrix4(matrix.makeTranslation(spaceX[i], 0, -shift-0.75));
        mergedGeometry = THREE.BufferGeometryUtils.mergeBufferGeometries([mergedGeometry, geometry]);


        geometry2.applyMatrix4(matrix.makeRotationX(-Math.PI / 2)).applyMatrix4(matrix.makeTranslation(spaceX[i], 0, zLength - shift+0.75));
        mergedGeometry = THREE.BufferGeometryUtils.mergeBufferGeometries([mergedGeometry, geometry2]);
    }

    for (let i = 0; i < numberInZ; i++) {
        let geometry = new THREE.TextBufferGeometry(`${i + 1}`, geoProp);
        let geometry2 = geometry.clone();
        geometry.applyMatrix4(matrix.makeRotationX(Math.PI / 2)).applyMatrix4(matrix.makeTranslation(-shift - 1, 0, spaceZ[i]));
        mergedGeometry = THREE.BufferGeometryUtils.mergeBufferGeometries([mergedGeometry, geometry]);
        
        geometry2.applyMatrix4(matrix.makeRotationX(-Math.PI / 2)).applyMatrix4(matrix.makeTranslation(xLength -shift +0.75, 0, spaceZ[i]));
        mergedGeometry = THREE.BufferGeometryUtils.mergeBufferGeometries([mergedGeometry, geometry2]);
    }
    this.letters = new THREE.Mesh(mergedGeometry, new THREE.MeshBasicMaterial({ color: 0x333333, opacity: 0.3, transparent: true }));

    //#region Creating Axess
    /*this.axes = new THREE.Mesh();
    this.axes.add(new THREE.ArrowHelper(xDirection, new THREE.Vector3(), this.lineLengthInX, 0xff0000, 0.05 * this.lineLengthInX));//x-Axis
    this.axes.add(new THREE.ArrowHelper(zDirection, new THREE.Vector3(), this.lineLengthInZ, 0x00ff00, 0.05 * this.lineLengthInZ));//y-Axis(In UI)
    this.axes.add(new THREE.ArrowHelper(new THREE.Vector3(0, 1, 0), new THREE.Vector3(), height + shift, 0x0000ff, 0.05 * (height + shift)));//z-Axis(In UI)
    */
    this.axes = new THREE.AxesHelper(shift);
    //let namesGeometry = new THREE.TextBufferGeometry('a', geoProp);

    let namesGeometry = new THREE.TextBufferGeometry(`X`, geoProp);
    namesGeometry.applyMatrix4(matrix.makeTranslation(shift, 0, 0));
    //namesGeometry = THREE.BufferGeometryUtils.mergeBufferGeometries([namesGeometry, xGeometry]);

    let yGeometry = new THREE.TextBufferGeometry(`Y`, geoProp);
    yGeometry.applyMatrix4(matrix.makeTranslation(0, 0, shift));
    namesGeometry = THREE.BufferGeometryUtils.mergeBufferGeometries([namesGeometry, yGeometry]);

    let zGeometry = new THREE.TextBufferGeometry(`Z`, geoProp);
    zGeometry.applyMatrix4(matrix.makeTranslation(0, shift, 0));
    namesGeometry = THREE.BufferGeometryUtils.mergeBufferGeometries([namesGeometry, zGeometry]);

    let lettersMesh = new THREE.Mesh(namesGeometry, new THREE.MeshBasicMaterial({ color: 0x000000, vertexColors: true}));
    this.axes.add(lettersMesh);
    
    this.axes.position.set(-shift * 1.5, 0, -shift * 1.5);
    //this.axes.position.set(-35, -15, -60);
    //#endregion
}

function Line(startPoint, endPoint, length, direction) {
    this.startPoint = startPoint || new THREE.Vector3(0, 0, 0);
    this.endPoint = endPoint || new THREE.Vector3(1, 0, 0);
    this.length = length || 50;
    this.direction = direction || new THREE.Vector3(1, 0, 0);
    this.material = new THREE.LineDashedMaterial({
        color: 0x333333,
        opacity: 0.3,
        transparent: true,
        gapSize: 0.5,
        dashSize: 0.5,
        scale: 1
    });
    this.geometry = new THREE.BufferGeometry().setFromPoints([this.startPoint, this.endPoint]);
    this.line = new THREE.Line(this.geometry, this.material);
    this.line.computeLineDistances();
}

function sum(a, b) {
    return a + b;
}