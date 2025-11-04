# Maze Completion Tool

This tool will help you automatically complete your maze by reading your reference image and placing wall cubes where needed!

## 📋 How to Use

### Setup (One Time)

1. **Add the Script to Your Scene**
   - Create an empty GameObject in your scene (Right-click in Hierarchy → Create Empty)
   - Name it "MazeGenerator" or similar
   - Add the `MazeFromImageGenerator` component to it

2. **Configure the Generator**
   - **Maze Texture**: Drag your `35 by 25 orthogonal maze.png` from the Textures folder
   - **Wall Prefab**: (Optional) If you want to use a specific prefab for walls, assign it here. Otherwise, it will create simple cubes
   - **Cell Size**: Set this to match your existing wall spacing (default is 1.0)
   - **Start Offset**: Adjust this to align with where your existing walls start in the world

3. **Adjust Settings**
   - **Wall Threshold**: 0.5 works for most images (dark pixels = walls, light = paths)
   - **Skip Existing Walls**: Keep this checked! It will avoid placing walls where you already have them
   - **Existing Wall Check Radius**: 0.3 is usually good for detecting existing walls

### Generate the Maze

1. Click the **"Generate Missing Walls from Image"** button
2. The script will:
   - Read your reference image
   - Detect where walls should be (dark pixels)
   - Skip positions where walls already exist
   - Create new wall cubes only where needed

3. Review the results in the Scene view

### Useful Features

- **Show Preview**: Enable this to see red wireframes showing where walls will be placed (in Scene view with gizmos enabled)
- **Analyze Current Scene**: Click this button to count how many wall objects are currently in your scene
- **Clear Generated Walls**: If you need to start over, this removes all walls created by this tool

## 🎨 Tips

### Aligning with Your Existing Walls

1. Look at one of your existing walls and note its position
2. If your first wall is at position (0, 0, 0), set Start Offset to (0, 0, 0)
3. If your walls are 1 unit apart, set Cell Size to 1.0
4. The maze image should align with your scene coordinates

### Using Your Own Wall Prefab

If you want walls to match your existing style:
1. Select one of your existing walls
2. Drag it into your Project panel to create a prefab
3. Assign this prefab to the "Wall Prefab" field

### Fine-Tuning

- If walls are being placed at existing positions, increase the "Existing Wall Check Radius"
- If the maze is rotated wrong, you may need to rotate the image or adjust the generation code
- If walls are offset, adjust the "Start Offset" value

## 🔧 Troubleshooting

**Problem**: Walls are being placed in wrong positions
- **Solution**: Check your Start Offset and Cell Size values. They need to match your existing maze grid.

**Problem**: Script is creating walls where walls already exist
- **Solution**: Increase the "Existing Wall Check Radius" or make sure existing walls have "Wall" in their name.

**Problem**: No walls are being generated
- **Solution**: Check that your maze texture is assigned and the texture import settings allow it to be read (Read/Write Enabled in import settings).

**Problem**: All positions are being skipped
- **Solution**: Your "Skip Existing Walls" might be detecting too many colliders. Disable it temporarily or adjust the check radius.

## 📝 How It Works

1. The script reads your reference image pixel by pixel
2. Dark pixels (below the threshold) are marked as walls
3. For each wall position, it converts the image coordinates to world coordinates
4. Before placing a wall, it checks if one already exists there
5. It only creates walls in empty positions

## 🎯 Expected Results

Based on your 35×25 maze image:
- The script will detect approximately 300-600 wall positions (depending on your maze design)
- It will skip any positions where you've already manually placed walls
- You'll get a complete maze matching your reference image!

## 🚀 Next Steps

After generating your maze:
1. You can manually adjust individual walls if needed
2. Merge the generated walls with your existing walls under a single parent
3. Consider adding collision, materials, or other gameplay elements
4. Test your maze!

---

**Note**: Make sure to save your scene before generating, in case you need to undo changes!
