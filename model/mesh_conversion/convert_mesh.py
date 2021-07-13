import bpy
import json

file = json.load(open("mesh.json", "r"))

mesh = bpy.data.meshes.new("hand_mesh")
verts = [(v[0], v[1], v[2]) for v in file["verts"]]
edges = []
faces = [(i[0], i[1], i[2]) for i in file["faces"]]

mesh.from_pydata(verts, edges, faces)

armature = bpy.data.armatures.new("hand_armature")

mesh_object = bpy.data.objects.new("hand_obj", mesh)
armature_object = bpy.data.objects.new("armature_obj", armature)

collection = bpy.data.collections.new("hand_collection")
bpy.context.scene.collection.children.link(collection)

collection.objects.link(mesh_object)
collection.objects.link(armature_object)

bpy.ops.object.select_all(action='DESELECT')
armature_object.select_set(True)
bpy.context.view_layer.objects.active = armature_object
bpy.ops.object.mode_set(mode='EDIT', toggle=False)
edit_bones = armature_object.data.edit_bones
bones = armature_object.data.bones
pose_bones = armature_object.pose.bones

joints = [(j[0], j[1], j[2]) for j in file["joints"]]

parents = file["parents"]
labels = file["labels"]
bones = []

for j in range(len(parents)):
    bone = edit_bones.new(labels[j])
    bone.tail = joints[j]
    if parents[j] == -1: 
        # this is the root bone.
        bone.head = (joints[j][0]-0.01, joints[j][1], joints[j][2])
    else:
        bone.head = joints[parents[j]]
        bone.parent = bones[parents[j]]
    bones.append(bone)

bpy.ops.object.mode_set(mode='OBJECT', toggle=False)



