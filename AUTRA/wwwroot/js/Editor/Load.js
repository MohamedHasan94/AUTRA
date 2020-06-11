let loader = new THREE.FontLoader();
let myFont;
let loadMaterial = new THREE.MeshBasicMaterial({ color: 0xcc0000, transparent: true, opacity: 0.3, side: THREE.DoubleSide });
let fontMaterial = new THREE.MeshBasicMaterial({ color: 0x000000 });
loader.load('../lib/three.js/helvetiker_regular.typeface.json', function (font) {
    myFont = font;
    loader = null;
});
    
class Load {
    constructor(magnitude, pattern) {
        this.magnitude = magnitude * -1;
        this.pattern = pattern;
    }
}

class LineLoad extends Load {
    constructor(magnitude, pattern) {
        super(magnitude, pattern);
    }
    render(beam) {

        let mesh = new THREE.Mesh(new THREE.PlaneBufferGeometry(beam.data.length, this.magnitude, 1, 1)
            , loadMaterial.clone());

        let textGeometry = new THREE.TextBufferGeometry(`${this.magnitude * -1}`, {
            font: myFont,
            size: 0.5,
            height: 0,
            curveSegments: 3,
            bevelEnabled: false
        });
        let text = new THREE.Mesh(textGeometry, fontMaterial);
        mesh.add(text);

        mesh.position.copy(beam.visual.mesh.position);
        mesh.position.add(beam.visual.direction.clone().multiplyScalar(0.5 * beam.data.length));
        mesh.position.y += 0.5 * this.magnitude * -1;
        mesh.rotateY(Math.PI / 2 - beam.visual.mesh.rotation.y);
        return mesh;
    }
}


let dir = new THREE.Vector3(0, -1, 0);
class PointLoad extends Load {
    constructor(magnitude, pattern) {
        super(magnitude, pattern);
    }
    render(position) {
        position.y += -0.5 * this.magnitude;
        let arrow = new THREE.ArrowHelper(dir, position, -0.5 * this.magnitude, 0xcc00ff);
        let textGeometry = new THREE.TextBufferGeometry(`${this.magnitude * -1}`, {
            font: myFont,
            size: 0.3,
            height: 0,
            curveSegments: 3,
            bevelEnabled: false
        });
        let text = new THREE.Mesh(textGeometry, fontMaterial);
        text.rotateX(Math.PI);
        arrow.add(text);
        return arrow;
    }
}